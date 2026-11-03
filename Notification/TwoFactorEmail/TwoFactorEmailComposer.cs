using System.Net;
using System.Net.Mail;
using Zuhid.Notification.Shared;

namespace Zuhid.Notification.TwoFactorEmail;

public class TwoFactorEmailComposer() : BaseComposer("TwoFactorEmail")
{
    public virtual async Task<MailMessage> Compose(TwoFactorEmailMessage message)
    {
        var subject = "Your verification code";
        var style = await ReadTemplate("TwoFactorEmail.css");
        var body = (await ReadTemplate("TwoFactorEmail.html"))
            .Replace("{{email}}", message.Email ?? string.Empty)
            .Replace("{{token}}", WebUtility.HtmlEncode(message.Token));
        var mailMessage = new MailMessage
        {
            Subject = subject,
            Body = await CreateHtmlAsync(body, style),
            IsBodyHtml = true
        };
        mailMessage.To.Add(message.Email!);
        return mailMessage;
    }
}
