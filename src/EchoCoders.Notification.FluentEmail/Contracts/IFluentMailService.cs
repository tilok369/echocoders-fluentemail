using EchoCoders.Notification.FluentEmail.Models;

namespace EchoCoders.Notification.FluentEmail.Contracts;

public interface IFluentMailService
{
    IFluentMailService Setup(string senderAddress, List<string> toAddresses, List<string> ccAddresses, List<string> bccAddresses, bool htmlSupported = true);
    IFluentMailService FormatSubject(string rawSubject, string pattern, Dictionary<string, string> variables);
    IFluentMailService FormatBody(string rawBody, string pattern, Dictionary<string, string> variables);
    MailResponse Send();
    Task<MailResponse> SendAsync();
}