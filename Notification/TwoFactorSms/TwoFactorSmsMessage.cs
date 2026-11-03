using Zuhid.Notification.Shared;

namespace Zuhid.Notification.TwoFactorSms;

public class TwoFactorSmsMessage : IMessage
{
    public required string Phone { get; set; }
    public required string Token { get; set; }
}
