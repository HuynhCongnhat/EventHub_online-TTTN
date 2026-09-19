using EventHub.Web.Models.Auth;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity.Data;
using System.Net.Http.Json;
using ForgotPasswordRequestModel = EventHub.Web.Models.Auth.ForgotPasswordRequest;
using ForgotPasswordResponseModel = EventHub.Web.Models.Auth.ForgotPasswordResponse;
using LoginRequestModel = EventHub.Web.Models.Auth.LoginRequest;
using LoginResponseModel = EventHub.Web.Models.Auth.LoginResponse;
using RegisterRequestModel = EventHub.Web.Models.Auth.RegisterRequest;
using ResetPasswordRequestModel = EventHub.Web.Models.Auth.ResetPasswordRequest;
using ResetPasswordResponseModel = EventHub.Web.Models.Auth.ResetPasswordResponse;

namespace EventHub.Web.Services
{
    public class AuthApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient Client => _httpClientFactory.CreateClient("AuthService");


        public async Task<(bool Success, string Message)> RegisterAsync(
        RegisterRequestModel request)
        {
            try
            {
                var response = await Client.PostAsJsonAsync(
                    "/api/Auth/register",
                    request);

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Đăng ký tài khoản thành công.");
                }

                var error = await response.Content.ReadAsStringAsync();

                try
                {
                    using var json =
                        System.Text.Json.JsonDocument.Parse(error);

                    if (json.RootElement.TryGetProperty(
                            "message",
                            out var message))
                    {
                        return (
                            false,
                            message.GetString()
                                ?? "Đăng ký tài khoản thất bại."
                        );
                    }
                }
                catch
                {
                }

                return (
                    false,
                    string.IsNullOrWhiteSpace(error)
                        ? "Đăng ký tài khoản thất bại."
                        : error
                );
            }
            catch (HttpRequestException)
            {
                return (
                    false,
                    "Không thể kết nối ."
                );
            }
            catch (Exception ex)
            {
                return (
                    false,
                    $"Đã xảy ra lỗi: {ex.Message}"
                );
            }
        }

        public async Task<(bool Success, string Message)> VerifyEmailAsync( VerifyEmailRequest request)
        {
            try
            {
                var response = await Client.PostAsJsonAsync(
                    "/api/Auth/verify-email",
                    request);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content
                        .ReadFromJsonAsync<VerifyEmailResponse>();

                    return (
                        true,
                        data?.Message
                            ?? "Xác thực email thành công."
                    );
                }

                var error = await response.Content.ReadAsStringAsync();

                try
                {
                    using var json =
                        System.Text.Json.JsonDocument.Parse(error);

                    if (json.RootElement.TryGetProperty(
                            "message",
                            out var message))
                    {
                        return (
                            false,
                            message.GetString()
                                ?? "Mã xác thực không hợp lệ."
                        );
                    }
                }
                catch
                {
                    // Response không phải JSON
                }

                return (
                    false,
                    string.IsNullOrWhiteSpace(error)
                        ? "Mã xác thực không hợp lệ hoặc đã hết hạn."
                        : error
                );
            }
            catch (HttpRequestException)
            {
                return (
                    false,
                    "Không thể kết nối đến AuthService. Hãy kiểm tra AuthService có đang chạy hay không."
                );
            }
            catch (Exception ex)
            {
                return (
                    false,
                    $"Đã xảy ra lỗi: {ex.Message}"
                );
            }
        }


        public async Task<(bool Success, string Message)> ResendVerificationCodeAsync(
    ResendVerificationCodeRequest request)
        {
            try
            {
                var response = await Client.PostAsJsonAsync(
                    "/api/Auth/resend-verification-code",
                    request);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content
                        .ReadFromJsonAsync<ResendVerificationCodeResponse>();

                    return (
                        true,
                        data?.Message
                            ?? "Mã xác thực mới đã được gửi đến email."
                    );
                }

                var error = await response.Content.ReadAsStringAsync();

                try
                {
                    using var json =
                        System.Text.Json.JsonDocument.Parse(error);

                    if (json.RootElement.TryGetProperty(
                            "message",
                            out var message))
                    {
                        return (
                            false,
                            message.GetString()
                                ?? "Không thể gửi lại mã xác thực."
                        );
                    }
                }
                catch
                {
                    // Response không phải JSON
                }

                return (
                    false,
                    string.IsNullOrWhiteSpace(error)
                        ? "Không thể gửi lại mã xác thực."
                        : error
                );
            }
            catch (HttpRequestException)
            {
                return (
                    false,
                    "Không thể kết nối ."
                );
            }
            catch (Exception ex)
            {
                return (
                    false,
                    $"Đã xảy ra lỗi: {ex.Message}"
                );
            }
        }


        public async Task<(bool Success, LoginResponseModel? Data, string Message)> LoginAsync(
            LoginRequestModel request)
        {
            try
            {
                var response = await Client.PostAsJsonAsync(
                    "/api/Auth/login",
                    request);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content
                        .ReadFromJsonAsync<LoginResponseModel>();

                    if (data != null)
                    {
                        return (
                            true,
                            data,
                            "Đăng nhập thành công."
                        );
                    }

                    return (
                        false,
                        null,
                        "AuthService không trả về dữ liệu đăng nhập."
                    );
                }

                var error = await response.Content.ReadAsStringAsync();

                try
                {
                    using var json = System.Text.Json.JsonDocument.Parse(error);

                    if (json.RootElement.TryGetProperty("message", out var message))
                    {
                        return (
                            false,
                            null,
                            message.GetString() ?? "Email hoặc mật khẩu không chính xác."
                        );
                    }
                }
                catch
                {
                    // Response không phải JSON thì xử lý bên dưới
                }

                return (
                    false,
                    null,
                    string.IsNullOrWhiteSpace(error)
                        ? "Email hoặc mật khẩu không chính xác."
                        : error
                );
            }
            catch (HttpRequestException)
            {
                return (
                    false,
                    null,
                    "Không thể kết nối đến AuthService. Hãy kiểm tra AuthService có đang chạy hay không."
                );
            }
            catch (Exception ex)
            {
                return (
                    false,
                    null,
                    $"Đã xảy ra lỗi: {ex.Message}"
                );
            }
        }

        public async Task<(bool Success, RefreshTokenResponse? Data, string Message)> RefreshTokenAsync(
            string refreshToken)
        {
            try
            {
                var request = new RefreshTokenRequest
                {
                    RefreshToken = refreshToken
                };

                var response = await Client.PostAsJsonAsync(
                    "/api/Auth/refresh-token",
                    request);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content
                        .ReadFromJsonAsync<RefreshTokenResponse>();

                    if (data != null)
                    {
                        return (
                            true,
                            data,
                            "Làm mới Access Token thành công."
                        );
                    }

                    return (
                        false,
                        null,
                        "AuthService không trả về dữ liệu Refresh Token."
                    );
                }

                var error = await response.Content.ReadAsStringAsync();

                return (
                    false,
                    null,
                    string.IsNullOrWhiteSpace(error)
                        ? "Refresh Token không hợp lệ hoặc đã hết hạn."
                        : error
                );
            }
            catch (HttpRequestException)
            {
                return (
                    false,
                    null,
                    "Không thể kết nối đến AuthService."
                );
            }
            catch (Exception ex)
            {
                return (
                    false,
                    null,
                    $"Đã xảy ra lỗi: {ex.Message}"
                );
            }
        }

        public async Task<(bool Success, string Message)> ForgotPasswordAsync(
            ForgotPasswordRequestModel request)
        {
            try
            {
                var response = await Client.PostAsJsonAsync(
                    "/api/Auth/forgot-password",
                    request);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content
                        .ReadFromJsonAsync<ForgotPasswordResponseModel>();

                    return (
                        true,
                        data?.Message
                            ?? "Nếu email tồn tại trong hệ thống, hướng dẫn đặt lại mật khẩu sẽ được gửi đến email của bạn."
                    );
                }

                var error = await response.Content.ReadAsStringAsync();

                return (
                    false,
                    string.IsNullOrWhiteSpace(error)
                        ? "Không thể gửi yêu cầu đặt lại mật khẩu."
                        : error
                );
            }
            catch (HttpRequestException)
            {
                return (
                    false,
                    "Không thể kết nối đến AuthService. Hãy kiểm tra AuthService có đang chạy hay không."
                );
            }
            catch (Exception ex)
            {
                return (
                    false,
                    $"Đã xảy ra lỗi: {ex.Message}"
                );
            }
        }

        public async Task<(bool Success, string Message)> ResetPasswordAsync(
            ResetPasswordRequestModel request)
        {
            try
            {
                var response = await Client.PostAsJsonAsync(
                    "/api/Auth/reset-password",
                    request);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content
                        .ReadFromJsonAsync<ResetPasswordResponseModel>();

                    return (
                        true,
                        data?.Message
                            ?? "Đặt lại mật khẩu thành công."
                    );
                }

                var error = await response.Content.ReadAsStringAsync();

                return (
                    false,
                    string.IsNullOrWhiteSpace(error)
                        ? "Không thể đặt lại mật khẩu."
                        : error
                );
            }
            catch (HttpRequestException)
            {
                return (
                    false,
                    "Không thể kết nối đến AuthService. Hãy kiểm tra AuthService có đang chạy hay không."
                );
            }
            catch (Exception ex)
            {
                return (
                    false,
                    $"Đã xảy ra lỗi: {ex.Message}"
                );
            }
        }
    }


}
