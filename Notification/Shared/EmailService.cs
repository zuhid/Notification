using System.Net.Mail;

namespace Zuhid.Notification.Shared;

public class EmailService(AppSetting appSetting, ISmtpClient smtpClient, ILogger<EmailService> logger)
{
    public virtual async Task SendEmailAsync(MailMessage mailMessage)
    {
        var retryCount = appSetting.Smtp.RetryCount;
        var delay = appSetting.Smtp.RetryInterval;
        for (var i = 0; i < retryCount; i++)
        {
            try
            {
                if (mailMessage.From == null || string.IsNullOrWhiteSpace(mailMessage.From.Address))
                {
                    mailMessage.From = new MailAddress(appSetting.Smtp.From);
                }
                await smtpClient.SendMailAsync(mailMessage);
                return;
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, $"{mailMessage.Subject}: {i}");
                if (i == retryCount - 1)
                {
                    throw;
                }
                await Task.Delay(delay);
                delay *= 2;
            }
        }
    }

    public virtual async Task SendTextAsync(string phone, string body)
    {
        var mailMessage = new MailMessage
        {
            Subject = $"{phone}",
            Body = body,
            IsBodyHtml = true
        };
        mailMessage.To.Add("phone@test.com");
        await SendEmailAsync(mailMessage);
    }
}
