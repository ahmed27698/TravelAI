using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace TravelAI.Services;

public class EmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendContactEmailAsync(string senderName, string senderEmail, string subject, string message)
    {
        var smtpUser = _config["Smtp:Username"];
        var smtpPass = _config["Smtp:Password"];
        var toEmail  = _config["Smtp:ToEmail"] ?? "ahmedsamir7685@gmail.com";

        if (string.IsNullOrWhiteSpace(smtpUser) || string.IsNullOrWhiteSpace(smtpPass))
        {
            _logger.LogWarning("SMTP not configured. Contact message from {Email} was not sent.", senderEmail);
            return;
        }

        var mime = new MimeMessage();
        mime.From.Add(new MailboxAddress(_config["Smtp:FromName"] ?? "TravelAI", smtpUser));
        mime.To.Add(MailboxAddress.Parse(toEmail));
        mime.ReplyTo.Add(new MailboxAddress(senderName, senderEmail));
        mime.Subject = $"[TravelAI Contact] {subject}";

        mime.Body = new TextPart("html")
        {
            Text = $"""
                <div style="font-family:Inter,sans-serif;max-width:560px;margin:0 auto;background:#0f172a;color:#e2e8f0;border-radius:12px;overflow:hidden;">
                  <div style="background:linear-gradient(135deg,#2563eb,#1d4ed8);padding:28px 32px;">
                    <h2 style="margin:0;color:#fff;font-size:20px;">New Contact Message</h2>
                    <p style="margin:4px 0 0;color:#bfdbfe;font-size:13px;">via TravelAI contact form</p>
                  </div>
                  <div style="padding:28px 32px;">
                    <table style="width:100%;border-collapse:collapse;font-size:14px;">
                      <tr><td style="padding:8px 0;color:#94a3b8;width:80px;">Name</td><td style="color:#e2e8f0;font-weight:600;">{System.Net.WebUtility.HtmlEncode(senderName)}</td></tr>
                      <tr><td style="padding:8px 0;color:#94a3b8;">Email</td><td><a href="mailto:{System.Net.WebUtility.HtmlEncode(senderEmail)}" style="color:#60a5fa;">{System.Net.WebUtility.HtmlEncode(senderEmail)}</a></td></tr>
                      <tr><td style="padding:8px 0;color:#94a3b8;">Subject</td><td style="color:#e2e8f0;">{System.Net.WebUtility.HtmlEncode(subject)}</td></tr>
                    </table>
                    <hr style="border:none;border-top:1px solid #1e293b;margin:16px 0;" />
                    <p style="color:#94a3b8;font-size:12px;margin-bottom:8px;">Message:</p>
                    <div style="background:#1e293b;border-radius:8px;padding:16px;color:#e2e8f0;font-size:14px;line-height:1.6;white-space:pre-wrap;">{System.Net.WebUtility.HtmlEncode(message)}</div>
                  </div>
                  <div style="padding:16px 32px;background:#1e293b;font-size:12px;color:#475569;">TravelAI · Reply directly to this email to respond to {System.Net.WebUtility.HtmlEncode(senderName)}</div>
                </div>
                """
        };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(
            _config["Smtp:Host"] ?? "smtp.gmail.com",
            int.Parse(_config["Smtp:Port"] ?? "587"),
            SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(smtpUser, smtpPass);
        await smtp.SendAsync(mime);
        await smtp.DisconnectAsync(true);
    }
}
