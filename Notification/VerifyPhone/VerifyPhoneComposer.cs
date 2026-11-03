using Zuhid.Notification.Shared;

namespace Zuhid.Notification.VerifyPhone;

public class VerifyPhoneComposer() : BaseComposer("VerifyPhone")
{
    public virtual Task<string> Compose(VerifyPhoneMessage message)
    {
        return Task.FromResult($"Your verification code is: {message.Token}");
    }
}
