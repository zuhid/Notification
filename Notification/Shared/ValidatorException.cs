namespace Zuhid.Notification.Shared;

public class ValidatorException(IEnumerable<string> messages) : ApplicationException(string.Join("; ", messages))
{
}
