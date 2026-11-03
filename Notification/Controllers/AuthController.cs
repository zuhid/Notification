using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zuhid.Notification.Shared;
using Zuhid.Notification.TwoFactorEmail;
using Zuhid.Notification.TwoFactorSms;
using Zuhid.Notification.VerifyEmail;
using Zuhid.Notification.VerifyPhone;

namespace Zuhid.Notification.Controllers;

[AllowAnonymous]
[ApiController]
[Route("[controller]")]
public class AuthController(NotificationQueue emailQueue) : ControllerBase
{
    [HttpPost("VerifyEmail")]
    public async Task VerifyEmail([FromBody] VerifyEmailMessage message) => await emailQueue.QueueMessage(message);

    [HttpPost("VerifyPhone")]
    public async Task VerifyPhone([FromBody] VerifyPhoneMessage message) => await emailQueue.QueueMessage(message);

    [HttpPost("SendTwoFactorEmail")]
    public async Task SendTwoFactorEmail([FromBody] TwoFactorEmailMessage message) => await emailQueue.QueueMessage(message);

    [HttpPost("SendTwoFactorSms")]
    public async Task SendTwoFactorSms([FromBody] TwoFactorSmsMessage message) => await emailQueue.QueueMessage(message);
}
