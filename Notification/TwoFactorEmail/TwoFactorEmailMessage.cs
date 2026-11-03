using Zuhid.Notification.Shared;

namespace Zuhid.Notification.TwoFactorEmail;

public class TwoFactorEmailMessage : IMessage
{
    public required string Email { get; set; }
    public required string Token { get; set; }
}
