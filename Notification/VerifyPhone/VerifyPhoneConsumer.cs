using Zuhid.Notification.Shared;

namespace Zuhid.Notification.VerifyPhone;

public class VerifyPhoneConsumer(EmailService emailService, VerifyPhoneComposer composer) : IConsumer<VerifyPhoneMessage>
{
    public async Task ConsumeAsync(VerifyPhoneMessage message, CancellationToken stoppingToken)
    {
        var body = await composer.Compose(message);
        await emailService.SendTextAsync(message.Phone, body);
    }
}
