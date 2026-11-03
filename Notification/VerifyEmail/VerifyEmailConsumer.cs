using Zuhid.Notification.Shared;

namespace Zuhid.Notification.VerifyEmail;

public class VerifyEmailConsumer(EmailService emailService, VerifyEmailComposer composer) : IConsumer<VerifyEmailMessage>
{
    public async Task ConsumeAsync(VerifyEmailMessage message, CancellationToken stoppingToken)
    {
        var mailMessage = await composer.Compose(message);
        await emailService.SendEmailAsync(mailMessage);
    }
}
