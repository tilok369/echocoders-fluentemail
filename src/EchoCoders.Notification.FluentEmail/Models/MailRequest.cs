namespace EchoCoders.Notification.FluentEmail.Models;

public record MailRequest(
    List<string> ToAddresses,
    List<string> CcAddresses,
    List<string> BccAddresses,
    string SenderAddress,
    bool HtmlSupported = true)
{
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}