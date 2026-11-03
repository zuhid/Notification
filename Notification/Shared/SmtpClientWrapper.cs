using System.Net.Mail;

namespace Zuhid.Notification.Shared;

public class SmtpClientWrapper(AppSetting appSetting) : ISmtpClient
{
    private readonly SmtpClient _client = new(appSetting.Smtp.Host, appSetting.Smtp.Port);

    public Task SendMailAsync(MailMessage message)
    {
        return _client.SendMailAsync(message);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _client.Dispose();
    }
}
