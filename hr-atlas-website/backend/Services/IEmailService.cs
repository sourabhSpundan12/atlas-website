using System.Net;
using System.Net.Mail;
using HrAtlas.SaaS.Data;
using Microsoft.Extensions.Options;

namespace HrAtlas.SaaS.Services;

public interface IEmailService
{
    Task SendOtpEmailAsync(string recipientEmail, string otpCode, string purpose);
}

public class EmailService : IEmailService
{
    private readonly SmtpSettings _smtpSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<SmtpSettings> smtpSettings, ILogger<EmailService> logger)
    {
        _smtpSettings = smtpSettings.Value;
        _logger = logger;
    }

    public async Task SendOtpEmailAsync(string recipientEmail, string otpCode, string purpose)
    {
        if (string.IsNullOrWhiteSpace(_smtpSettings.Host) ||
            string.IsNullOrWhiteSpace(_smtpSettings.Username) ||
            string.IsNullOrWhiteSpace(_smtpSettings.Password))
        {
            _logger.LogWarning("SMTP credentials are not configured in appsettings.json. Simulating email dispatch for [{Email}] with OTP [{Code}].", recipientEmail, otpCode);
            return;
        }

        try
        {
            using var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
            {
                EnableSsl = _smtpSettings.EnableSsl,
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password)
            };

            var subject = purpose == "PasswordReset"
                ? "HR Atlas - Password Reset Verification Code"
                : "HR Atlas - Your Work Email Verification Code";

            var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
  <meta charset='utf-8'>
  <style>
    body {{ font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif; background-color: #f8fafc; margin: 0; padding: 24px; }}
    .container {{ max-width: 520px; margin: 0 auto; background-color: #ffffff; border: 1px solid #e2e8f0; border-radius: 12px; overflow: hidden; }}
    .header {{ background: linear-gradient(135deg, #1e3a8a 0%, #2563eb 100%); padding: 28px 24px; text-align: center; color: #ffffff; }}
    .header h1 {{ margin: 0; font-size: 24px; font-weight: 800; letter-spacing: -0.5px; }}
    .header p {{ margin: 6px 0 0; font-size: 13px; opacity: 0.9; }}
    .content {{ padding: 32px 24px; text-align: center; }}
    .greeting {{ font-size: 16px; color: #0f172a; margin-bottom: 12px; font-weight: 600; }}
    .instruction {{ font-size: 14px; color: #475569; line-height: 1.5; margin-bottom: 24px; }}
    .otp-box {{ background-color: #eff6ff; border: 1.5px dashed #3b82f6; border-radius: 10px; padding: 18px 24px; font-size: 34px; font-weight: 800; letter-spacing: 8px; color: #1e3a8a; margin: 0 auto 24px; display: inline-block; }}
    .expiry {{ font-size: 13px; color: #ef4444; font-weight: 600; margin-bottom: 24px; }}
    .footer {{ background-color: #f8fafc; border-top: 1px solid #e2e8f0; padding: 16px 24px; text-align: center; font-size: 12px; color: #94a3b8; }}
  </style>
</head>
<body>
  <div class='container'>
    <div class='header'>
      <h1>HR Atlas</h1>
      <p>Next-Gen Enterprise Workforce Management</p>
    </div>
    <div class='content'>
      <div class='greeting'>Security Verification Code</div>
      <div class='instruction'>
        Use the single-use 6-digit code below to complete your { (purpose == "PasswordReset" ? "password recovery" : "organisation workspace setup") }.
      </div>
      <div class='otp-box'>{otpCode}</div>
      <div class='expiry'>Expires in 10 minutes</div>
      <p style='font-size: 12px; color: #64748b;'>If you did not request this verification code, please ignore this email or contact support@hratlas.com.</p>
    </div>
    <div class='footer'>
      &copy; {DateTime.UtcNow.Year} HR Atlas Technologies Inc. All rights reserved.
    </div>
  </div>
</body>
</html>";

            var sender = string.IsNullOrWhiteSpace(_smtpSettings.SenderEmail) ? _smtpSettings.Username : _smtpSettings.SenderEmail;
            using var mailMessage = new MailMessage
            {
                From = new MailAddress(sender, _smtpSettings.SenderName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            mailMessage.To.Add(recipientEmail);

            await client.SendMailAsync(mailMessage);
            _logger.LogInformation("Successfully sent OTP email to [{Email}] via SMTP [{Host}].", recipientEmail, _smtpSettings.Host);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to dispatch email to [{Email}] via SMTP.", recipientEmail);
            throw new InvalidOperationException($"Failed to send email verification: {ex.Message}", ex);
        }
    }
}

