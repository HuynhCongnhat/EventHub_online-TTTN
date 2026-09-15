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

namespace EventHub.AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(AuthDbContext context,IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
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

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Đăng ký tài khoản thành công.",
            userId = user.Id,
            email = user.Email,
            fullName = user.FullName,
            role = customerRole.Name
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

        var passwordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

        if (!passwordValid)
        {
            return Unauthorized(new
            {
                message = "email hoặc password không chính xác."
            });
        }

        var accessToken = GenerateJwtToken(user);

        return Ok(new
        {
            message = "Đăng nhập thành công",
            accessToken,
            userId = user.Id,
            email = user.Email
        });

    }

    // =========================
    // GET CURRENT USER
    // =========================

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


}