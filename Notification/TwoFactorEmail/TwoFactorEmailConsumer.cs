using Zuhid.Notification.Shared;

namespace Zuhid.Notification.TwoFactorEmail;

public class TwoFactorEmailConsumer(EmailService emailService, TwoFactorEmailComposer composer) : IConsumer<TwoFactorEmailMessage>
{
    public async Task ConsumeAsync(TwoFactorEmailMessage message, CancellationToken stoppingToken)
    {
        var mailMessage = await composer.Compose(message);
        await emailService.SendEmailAsync(mailMessage);
    }
}
