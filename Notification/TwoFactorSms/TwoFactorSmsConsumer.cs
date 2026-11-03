using Zuhid.Notification.Shared;

namespace Zuhid.Notification.TwoFactorSms;

public class TwoFactorSmsConsumer(EmailService emailService, TwoFactorSmsComposer composer) : IConsumer<TwoFactorSmsMessage>
{
    public async Task ConsumeAsync(TwoFactorSmsMessage message, CancellationToken stoppingToken)
    {
        var body = await composer.Compose(message);
        await emailService.SendTextAsync(message.Phone, body);
    }
}
