using System.Net.Mail;

namespace Zuhid.Notification.Shared;

public interface ISmtpClient : IDisposable
{
    Task SendMailAsync(MailMessage message);
}
