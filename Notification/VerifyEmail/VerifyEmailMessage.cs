using Zuhid.Notification.Shared;

namespace Zuhid.Notification.VerifyEmail;

public class VerifyEmailMessage : IMessage
{
    public required string Email { get; set; }
    public required string AppUrl { get; set; }
    public required string Token { get; set; }
}
