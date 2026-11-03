using System.Net;
using System.Net.Mail;
using Zuhid.Notification.Shared;

namespace Zuhid.Notification.VerifyEmail;

public class VerifyEmailComposer() : BaseComposer("VerifyEmail")
{
    public virtual async Task<MailMessage> Compose(VerifyEmailMessage message)
    {
        var subject = "Verify Email";
        var style = await ReadTemplate("VerifyEmail.css");
        var body = (await ReadTemplate("VerifyEmail.html"))
            .Replace("{{appUrl}}", message.AppUrl ?? string.Empty)
            .Replace("{{email}}", message.Email ?? string.Empty)
            .Replace("{{emailEncoded}}", WebUtility.UrlEncode(message.Email))
            .Replace("{{token}}", WebUtility.UrlEncode(message.Token));
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

