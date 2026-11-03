using Zuhid.Base;

namespace Zuhid.Notification.Shared;

public class AppSetting(IConfiguration configuration) : SettingBase(configuration)
{
    public SmtpSetting Smtp { get; init; } = default!;
    public class SmtpSetting
    {
        public string Host { get; init; } = default!;
        public int Port { get; init; }
        public string From { get; init; } = default!;
        public int RetryCount { get; init; }
        public TimeSpan RetryInterval { get; init; }
    }
}
