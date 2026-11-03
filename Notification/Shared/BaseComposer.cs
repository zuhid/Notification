namespace Zuhid.Notification.Shared;

public abstract class BaseComposer(string templateDir)
{
    protected virtual async Task<string> ReadTemplate(string filePath)
    {
        return await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, templateDir, filePath));
    }

    protected virtual async Task<string> CreateHtmlAsync(string body, string style = "")
    {
        var combinedStyle = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "Shared", "BaseComposer.css")) + style;
        var baseBody = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "Shared", "BaseComposer.html"));
        return baseBody
            .Replace("{{style}}", combinedStyle)
            .Replace("{{body}}", body);
    }
}
