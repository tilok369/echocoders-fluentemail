namespace EchoCoders.Notification.FluentEmail.Helper;

public class MailHelper
{
    public static string Construct(string rawString, string pattern, Dictionary<string, string> variables)
    {
        foreach (var item in variables)
            rawString = rawString.Replace($"{pattern}{item.Key}{pattern}", item.Value);
        return rawString.Replace("&lt;", "<").Replace("&gt;", ">")
            .Replace("%3C", "<").Replace("%3E", ">");
    }
}