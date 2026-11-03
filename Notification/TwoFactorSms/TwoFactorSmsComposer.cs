using Zuhid.Notification.Shared;

namespace Zuhid.Notification.TwoFactorSms;

public class TwoFactorSmsComposer() : BaseComposer("TwoFactorSms")
{
    public virtual Task<string> Compose(TwoFactorSmsMessage message)
    {
        return Task.FromResult($"Your two-factor authentication code is: {message.Token}");
    }
}
