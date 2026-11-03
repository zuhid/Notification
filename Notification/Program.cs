using Zuhid.Base;
using Zuhid.Notification.Shared;

namespace Zuhid.Notification;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var appSetting = new AppSetting(builder.Configuration);

        builder.AddServices(appSetting);
        builder.AddScopedByConvention(typeof(AppSetting).Assembly, ["Consumer", "Composer", "Repository", "Validator"]);

        builder.Services
            .AddHostedService<NotificationBackgroundService>()
            .AddScoped<EmailService>()
            .AddSingleton<ISmtpClient, SmtpClientWrapper>()
            .AddSingleton<NotificationQueue>();

        var app = builder.BuildServices(appSetting);
        app.Run();
    }
}
