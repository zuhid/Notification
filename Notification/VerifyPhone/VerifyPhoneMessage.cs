using Zuhid.Notification.Shared;

namespace Zuhid.Notification.VerifyPhone;

public class VerifyPhoneMessage : IMessage
{
    public required string Phone { get; set; }
    public required string Token { get; set; }
}
