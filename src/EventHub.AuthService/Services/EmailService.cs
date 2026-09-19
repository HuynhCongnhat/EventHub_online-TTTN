using MailKit.Security;
using MimeKit;
using MailKit.Net.Smtp;

namespace EventHub.AuthService.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(
            string toEmail,
            string subject,
            string htmlBody)
        {
            var host =
                _configuration["EmailSettings:Host"]
                ?? throw new InvalidOperationException(
                    "EmailSettings:Host chưa được cấu hình.");

            var portText =
                _configuration["EmailSettings:Port"]
                ?? "587";

            var username =
                _configuration["EmailSettings:Username"]
                ?? throw new InvalidOperationException(
                    "EmailSettings:Username chưa được cấu hình.");

            var password =
                _configuration["EmailSettings:Password"]
                ?? throw new InvalidOperationException(
                    "EmailSettings:Password chưa được cấu hình.");

            var fromName =
                _configuration["EmailSettings:FromName"]
                ?? "EventHub";

            if (!int.TryParse(portText, out var port))
            {
                throw new InvalidOperationException(
                    "EmailSettings:Port không hợp lệ.");
            }

            var email = new MimeMessage();

            email.From.Add(
                new MailboxAddress(fromName, username));

            email.To.Add(
                MailboxAddress.Parse(toEmail));

            email.Subject = subject;

            email.Body = new BodyBuilder
            {
                HtmlBody = htmlBody
            }.ToMessageBody();

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                host,
                port,
                SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(
                username,
                password);

            await smtp.SendAsync(email);

            await smtp.DisconnectAsync(true);
        }

        public async Task SendVerificationCodeAsync(
            string toEmail,
            string code)
        {
            var subject =
                "EventHub - Mã xác thực email";

            var htmlBody = $"""
                <!DOCTYPE html>
                <html lang="vi">
                <head>
                    <meta charset="UTF-8">
                </head>

                <body style="
                    margin:0;
                    padding:0;
                    background:#fff5f5;
                    font-family:Arial,sans-serif;
                ">

                    <div style="
                        max-width:600px;
                        margin:40px auto;
                        background:#ffffff;
                        border-radius:16px;
                        padding:32px;
                        box-shadow:0 4px 20px rgba(0,0,0,0.08);
                    ">

                        <h1 style="
                            color:#e96b6b;
                            margin-bottom:10px;
                        ">
                            🎟️ EventHub
                        </h1>

                        <h2 style="color:#333;">
                            Xác thực địa chỉ Email
                        </h2>

                        <p style="
                            color:#555;
                            font-size:16px;
                            line-height:1.6;
                        ">
                            Cảm ơn bạn đã đăng ký tài khoản
                            EventHub.
                        </p>

                        <p style="
                            color:#555;
                            font-size:16px;
                        ">
                            Mã xác thực email của bạn là:
                        </p>

                        <div style="
                            margin:25px 0;
                            padding:18px;
                            text-align:center;
                            background:#fff0f0;
                            border-radius:12px;
                            font-size:32px;
                            font-weight:bold;
                            letter-spacing:8px;
                            color:#e96b6b;
                        ">
                            {code}
                        </div>

                        <p style="
                            color:#777;
                            font-size:14px;
                            line-height:1.6;
                        ">
                            Mã này có hiệu lực trong
                            <strong>10 phút</strong>.
                        </p>

                        <p style="
                            color:#999;
                            font-size:13px;
                            margin-top:30px;
                        ">
                            Nếu bạn không thực hiện đăng ký này,
                            vui lòng bỏ qua email.
                        </p>

                    </div>

                </body>
                </html>
                """;

            await SendEmailAsync(
                toEmail,
                subject,
                htmlBody);
        }
    }
}