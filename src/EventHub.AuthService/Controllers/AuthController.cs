using EventHub.AuthService.Data;
using EventHub.AuthService.DTOs;
using EventHub.AuthService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using EventHub.AuthService.Services;

namespace EventHub.AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly EmailService _emailService;

    public AuthController(AuthDbContext context,IConfiguration configuration, EmailService emailService)
    {
        _context = context;
        _configuration = configuration;
        _emailService = emailService;
    }

    // register
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        // Kiểm tra dữ liệu đầu vào
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password) ||
            string.IsNullOrWhiteSpace(request.FullName))
        {
            return BadRequest(new
            {
                message = "Email, password và họ tên không được để trống."
            });
        }

        if(request.Password.Length < 6)
        {
            return BadRequest(new
            {
                message = "Mật khẩu phải có ít nhất 6 ký tự."
            });
        }

        var email = request.Email.Trim();

        // Kiểm tra email đã tồn tại
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);

        if (existingUser != null)
        {
            return Conflict(new
            {
                message = "Email đã được sử dụng."
            });
        }

        // Lấy Role Customer
        var customerRole = await _context.Roles
            .FirstOrDefaultAsync(r => r.Name == "Customer");

        if (customerRole == null)
        {
            return StatusCode(500, new
            {
                message = "Không tìm thấy role Customer."
            });
        }

        // Hash password bằng BCrypt
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // Tạo User
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email.Trim(),
            PasswordHash = passwordHash,
            FullName = request.FullName.Trim(),
            RoleId = customerRole.Id,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);



        // tạo otp xác thực
        var verificationCode = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();

        var verificationToken = new EmailVerificationToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Code = verificationCode,
            ExpiresAt = DateTime.UtcNow.AddMinutes(5),
            IsUsed = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.EmailVerificationTokens.Add(verificationToken);

        await _context.SaveChangesAsync();

        //verification tạm thời để test 

        // Gửi mã OTP xác thực email
        try
        {
            await _emailService.SendVerificationCodeAsync(
                user.Email,
                verificationCode);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Tạo tài khoản thành công nhưng không thể gửi email xác thực.",
                detail = ex.Message
            });
        }

        return Ok(new
        {
            message = "Đăng ký tài khoản thành công.",
            userId = user.Id,
            email = user.Email,
            fullName = user.FullName,
            role = customerRole.Name,
            emailVerified = user.EmailVerified,
            verificationCodeExpiresAt = verificationToken.ExpiresAt

        });
    }

    //verify email
    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail(
        VerifyEmailRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Code))
        {
            return BadRequest(new
            {
                message = "Email và mã xác nhận không được để trống."
            });
        }

        var email = request.Email.Trim();
        var code = request.Code.Trim();

        // OTP phải có đúng 6 chữ số
        if (code.Length != 6 ||
            !code.All(char.IsDigit))
        {
            return BadRequest(new
            {
                message = "Mã xác nhận phải gồm 6 chữ số."
            });
        }

        // Tìm User
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            return BadRequest(new
            {
                message = "Email hoặc mã xác nhận không hợp lệ."
            });
        }

        // Email đã xác thực
        if (user.EmailVerified)
        {
            return Ok(new
            {
                message = "Email đã được xác thực trước đó.",
                emailVerified = true
            });
        }

        // Tìm các OTP còn hiệu lực
        var verificationTokens =
            await _context.EmailVerificationTokens
                .Where(x =>
                    x.UserId == user.Id &&
                    !x.IsUsed &&
                    x.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

        EmailVerificationToken? validToken = null;

        foreach (var token in verificationTokens)
        {
            if (token.Code == code)
            {
                validToken = token;
                break;
            }
        }

        if (validToken == null)
        {
            return BadRequest(new
            {
                message =
                    "Mã xác nhận không hợp lệ hoặc đã hết hạn."
            });
        }

        // Đánh dấu OTP đã sử dụng
        validToken.IsUsed = true;

        // Xác thực email
        user.EmailVerified = true;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Xác thực email thành công.",
            email = user.Email,
            emailVerified = user.EmailVerified
        });
    }

    // RESEND VERIFICATION CODE

    [HttpPost("resend-verification-code")]
    public async Task<IActionResult> ResendVerificationCode(
        ResendVerificationCodeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest(new
            {
                message = "Email không được để trống."
            });
        }

        var email = request.Email.Trim();

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            return BadRequest(new
            {
                message = "Không tìm thấy tài khoản với email này."
            });
        }

        // Email đã xác thực rồi
        if (user.EmailVerified)
        {
            return BadRequest(new
            {
                message = "Email này đã được xác thực."
            });
        }

        // Vô hiệu hóa OTP cũ
        var oldTokens =
            await _context.EmailVerificationTokens
                .Where(x =>
                    x.UserId == user.Id &&
                    !x.IsUsed)
                .ToListAsync();

        foreach (var oldToken in oldTokens)
        {
            oldToken.IsUsed = true;
        }

        // Tạo OTP mới
        var verificationCode =
            RandomNumberGenerator.GetInt32(100000, 1000000)
                .ToString();

        var verificationToken = new EmailVerificationToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Code = verificationCode,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            IsUsed = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.EmailVerificationTokens.Add(
            verificationToken);

        await _context.SaveChangesAsync();

        try
        {
            await _emailService.SendVerificationCodeAsync(
                user.Email,
                verificationCode);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Không thể gửi lại mã xác thực qua email.",
                detail = ex.Message
            });
        }

        // Tạm thời trả OTP để test
        return Ok(new
        {
            message = "ã xác nhận mới đã được gửi đến email.",
            email = user.Email,
            verificationCodeExpiresAt =
                verificationToken.ExpiresAt
        });
    }



    //forgot password
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest(new
            {
                message = "Email không được để trống. "
            });
        }

        var email = request.Email.Trim();

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        // khong tiet lo mail co ton tai hay khong
        if (user == null)
        {
            return Ok(new
            {
                message = "Email đã tồn tại."
            });
        }

        //huy cac token cu chua su dung
        var oldTokens = await _context.PasswordResetTokens.Where(x => x.UserId == user.Id && !x.IsUsed).ToListAsync();

        foreach (var oldToken in oldTokens)
        {
            oldToken.IsUsed = true;
        }

        //tao token new
        //var resetToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

        //tạo otp
        var verificationCode = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();

        //hash token truoc khi luu data
        var tokenHash = BCrypt.Net.BCrypt.HashPassword(verificationCode);

        var passwordResetToken = new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = tokenHash,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            IsUsed = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.PasswordResetTokens.Add(passwordResetToken);

        await _context.SaveChangesAsync();

        try
        {
            await _emailService.SendVerificationCodeAsync(user.Email, verificationCode);
        }
        catch(Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Không thể gửi mã xác nhận đến mail",
                detail = ex.Message
            });
        }

        return Ok(new
        {
            message = "Nếu email tồn tại trong hệ thống, hướng dẫn đặt lại mật khẩu sẽ được gửi.",
            //tam request token de test
            //khi tich hop email that khong tra token nay ra API
            //resetToken
            email = user.Email,
            verificationCodeExpiresAt = passwordResetToken.ExpiresAt
        });

    }

    // reset password
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Token) ||
            string.IsNullOrWhiteSpace(request.NewPassword))
        {
            return BadRequest(new
            {
                message = "Email, token và mật khẩu mới không được để trống."
            });
        }

        if (request.NewPassword.Length < 6)
        {
            return BadRequest(new
            {
                message = "Mật khẩu mới phải có ít nhất 6 ký tự."
            });
        }

        var email = request.Email.Trim();

        var code = request.Token.Trim();

        if(code.Length != 6 || !code.All(char.IsDigit)){
            return BadRequest(new
            {
                message = "Mã xác nhận phải gồm 6 chữ số. "
            });
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            return BadRequest(new
            {
                message = "Mã xác nhận không hợp lệ hoặc đã hết hạn."
            });
        }

        //lấy các otp còn hiệu lực
        var resetTokens = await _context.PasswordResetTokens
            .Where(x =>
                x.UserId == user.Id &&
                !x.IsUsed &&
                x.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        PasswordResetToken? validToken = null;

        //kiểm tra otp
        foreach (var resetToken in resetTokens)
        {
            if (BCrypt.Net.BCrypt.Verify(code, resetToken.TokenHash))
            {
                validToken = resetToken;
                break;
            }
        }

        if (validToken == null)
        {
            return BadRequest(new
            {
                message = "Mã xác thực không hợp lệ hoặc đã hết hạn."
            });
        }

        // Cập nhật mật khẩu mới
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

        // Đánh dấu token đã sử dụng
        validToken.IsUsed = true;

        // vo hiệu hóa các otp khác
        foreach (var oldToken in resetTokens)
        {
            if (oldToken.Id != validToken.Id)
            {
                oldToken.IsUsed = true;
            }
        }
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Đặt lại mật khẩu thành công."
        });
    }

    //login
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new
            {
                message = "Email và password không để trống."
            });
        }

        var email = request.Email.Trim();

        var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            return Unauthorized(new
            {
                message = "Email hoặc password không chính xác."
            });
        }

        // kiemr tra email xác thực
        if (!user.EmailVerified)
        {
            return Unauthorized(new
            {
                message = "email chưa đúng",
                emailVerified = false,
                email = user.Email
            });
        }

        var passwordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

        if (!passwordValid)
        {
            return Unauthorized(new
            {
                message = "email hoặc password không chính xác."
            });
        }

        var accessToken = GenerateJwtToken(user);

        var refreshToken = GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            RevokedAt = null
        };

        _context.RefreshTokens.Add(refreshTokenEntity);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Đăng nhập thành công",
            accessToken,
            refreshToken,
            userId = user.Id,
            email = user.Email,
            fullName = user.FullName,
            role = user.Role?.Name ?? "Customer",
            emailVerification = user.EmailVerified
        });

    }

    //refresh
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return BadRequest(new
            {
                message = "Refresh token không được để trống."
            });
        }

        var token = await _context.RefreshTokens
            .Include(x => x.User)
            .ThenInclude(x => x!.Role)
            .FirstOrDefaultAsync(x => x.Token == refreshToken);

        if (token == null ||
            token.RevokedAt != null ||
            token.ExpiresAt <= DateTime.UtcNow ||
            token.User == null)
        {
            return Unauthorized(new
            {
                message = "Mã không hợp lệ hoặc đã hết hạn."
            });
        }

        var newAccessToken = GenerateJwtToken(token.User);

        return Ok(new
        {
            accessToken = newAccessToken,
            userId = token.User.Id,
            email = token.User.Email,
            fullName = token.User.FullName,
            role = token.User.Role?.Name ?? "Customer"
        });
    }

    // GET CURRENT USER

    [Authorize]
    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = User.FindFirstValue(ClaimTypes.Email);
        var fullName = User.FindFirstValue(ClaimTypes.Name);
        var role = User.FindFirstValue(ClaimTypes.Role);

        return Ok(new
        {
            userId,
            email,
            fullName,
            role
        });
    }

    //Generate JWT token
    private string GenerateJwtToken(User user)
    {
        var key = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException(
                "JWT Key chưa được cấu hình.");

        var issuer = _configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException(
                "JWT Issuer chưa được cấu hình.");

        var audience = _configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException(
                "JWT Audience chưa được cấu hình.");

        var expireMinutes = _configuration
            .GetValue<int>("Jwt:ExpireMinutes");

        // Các thông tin được lưu trong JWT
        var claims = new List<Claim>
        {
            new Claim(
                JwtRegisteredClaimNames.Sub,
                user.Id.ToString()),

            new Claim(
                JwtRegisteredClaimNames.Email,
                user.Email),

            new Claim(
                ClaimTypes.Name,
                user.FullName),

            new Claim(
                ClaimTypes.Role,
                user.Role?.Name ?? "Customer")
        };

        // Tạo Security Key
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key));

        // Thuật toán ký JWT
        var credentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256);

        // Tạo JWT
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expireMinutes),
            signingCredentials: credentials);

        // Chuyển JWT thành chuỗi
        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);

        return Convert.ToBase64String(randomBytes);
    }

}