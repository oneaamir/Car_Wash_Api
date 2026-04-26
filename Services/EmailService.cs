using System.Net;
using System.Net.Mail;
using CarWash.Backend.Services.Interfaces;

namespace CarWash.Backend.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = false)
    {
        var emailSection = _configuration.GetSection("Email");
        var isEnabled = bool.TryParse(emailSection["Enabled"], out var enabledValue) && enabledValue;

        if (!isEnabled)
        {
            _logger.LogInformation("Email notification skipped because email is disabled. To: {ToEmail}, Subject: {Subject}", toEmail, subject);
            return;
        }

        var smtpHost = emailSection["SmtpHost"];
        var smtpPort = int.TryParse(emailSection["SmtpPort"], out var port) ? port : 587;
        var fromEmail = emailSection["FromEmail"];
        var fromName = emailSection["FromName"] ?? "Car Wash System";
        var username = emailSection["Username"];
        var password = emailSection["Password"];

        if (string.IsNullOrWhiteSpace(smtpHost) ||
            string.IsNullOrWhiteSpace(fromEmail) ||
            string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password))
        {
            _logger.LogWarning("Email notification skipped because SMTP settings are incomplete. To: {ToEmail}, Subject: {Subject}", toEmail, subject);
            return;
        }

        using var smtpClient = new SmtpClient(smtpHost, smtpPort)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(username, password)
        };

        using var mailMessage = new MailMessage
        {
            From = new MailAddress(fromEmail, fromName),
            Subject = subject,
            Body = isHtml ? BuildHtmlTemplate(subject, body) : body,
            IsBodyHtml = isHtml
        };

        mailMessage.To.Add(toEmail);

        await smtpClient.SendMailAsync(mailMessage);

        _logger.LogInformation("Email notification sent successfully. To: {ToEmail}, Subject: {Subject}", toEmail, subject);
    }

    private static string BuildHtmlTemplate(string title, string content)
    {
        return $"""
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>{title}</title>
</head>
<body style="margin:0;padding:0;background-color:#f4f7fb;font-family:Arial,Helvetica,sans-serif;color:#1f2937;">
    <div style="max-width:640px;margin:32px auto;padding:0 16px;">
        <div style="background:#111827;color:#ffffff;padding:20px 24px;border-radius:12px 12px 0 0;">
            <h1 style="margin:0;font-size:24px;">Car Wash System</h1>
        </div>
        <div style="background:#ffffff;padding:32px 24px;border:1px solid #e5e7eb;border-top:none;border-radius:0 0 12px 12px;">
            <h2 style="margin-top:0;font-size:22px;color:#111827;">{title}</h2>
            <div style="font-size:16px;line-height:1.7;color:#374151;">
                {content}
            </div>
            <div style="margin-top:32px;padding-top:20px;border-top:1px solid #e5e7eb;font-size:13px;color:#6b7280;">
                This is an automated email from Car Wash System.
            </div>
        </div>
    </div>
</body>
</html>
""";
    }
}
