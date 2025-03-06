using System.Net;
using System.Net.Mail;
using EchoCoders.Notification.FluentEmail.Contracts;
using EchoCoders.Notification.FluentEmail.Helper;
using EchoCoders.Notification.FluentEmail.Models;

namespace EchoCoders.Notification.FluentEmail.Services;

public class FluentSmtpMailService: IFluentMailService
{
    private readonly SmtpMailSettings _smtpMailSettings;
    private MailRequest _mailRequest;
    
    public FluentSmtpMailService(SmtpMailSettings smtpMailSettings)
    {
        _smtpMailSettings = smtpMailSettings;
    }

    /// <summary>
    ///  Mail settings with sender, receivers (TO, CC and BCC) and whether mail supports HTML body or not
    /// </summary>
    /// <param name="senderAddress">From whom the mail will be sent</param>
    /// <param name="toAddresses">To whom the mail will be sent (To)</param>
    /// <param name="ccAddresses">To whom the mail will be sent (CC)</param>
    /// <param name="bccAddresses">To whom the mail will be sent (BCC)</param>
    /// <param name="htmlSupported">Whether mail body supports HTML or not (by default it is TRUE)</param>
    /// <returns>The parent object itself</returns>
    public IFluentMailService Setup(string senderAddress, List<string> toAddresses, List<string> ccAddresses, List<string> bccAddresses, bool htmlSupported = true)
    {
        _mailRequest = new MailRequest(
            toAddresses,
            ccAddresses,
            bccAddresses,
            senderAddress,
            htmlSupported
        );

        return this;
    }
    
    /// <summary>
    /// Construct Mail subject from rawSubject string to actual email subject based on provided pattern and variable dictionary
    /// Input: From ##Company##: OTP
    /// Output: From ABC Inc.: OTP
    /// </summary>
    /// <param name="rawSubject">Raw configured mail subject pattern, from above example: From ##Company##: OTP</param>
    /// <param name="pattern">Pattern enclosed string which need to be replaced with variables, from above example: ##</param>
    /// <param name="variables">key value pair, key (enclosed with pattern) will be replaced by value, from above example:  {Company, ABC Inc.}</param>
    /// <returns>Constructed actual subject and returns the parent object itself</returns>
    public IFluentMailService FormatSubject(string rawSubject, string pattern, Dictionary<string, string> variables)
    {
        _mailRequest.Subject = MailHelper.Construct(rawSubject, pattern, variables);

        return this;
    }
    
    /// <summary>
    /// Construct Mail body from rawBody string to actual email body based on provided pattern and variable dictionary
    /// Input: Hi ##Name##, How are you?
    /// Output: Hi David, How are you?
    /// </summary>
    /// <param name="rawBody">Raw configured mail body pattern, from above example: Hi ##Name##, How are you?</param>
    /// <param name="pattern">Pattern enclosed string which need to be replaced with variables, from above example: ##</param>
    /// <param name="variables">key value pair, key (enclosed with pattern) will be replaced by value, from above example:  {Name, David}</param>
    /// <returns>Constructed actual body and returns the parent object itself</returns>
    public IFluentMailService FormatBody(string rawBody, string pattern, Dictionary<string, string> variables)
    {
        _mailRequest.Body = MailHelper.Construct(rawBody, pattern, variables);

        return this;
    }
    
    /// <summary>
    /// This will send mail after initial setup in synchronous way
    /// </summary>
    /// <returns>MailResponse with response code and message</returns>
    public MailResponse Send()
    {
        using var smtpClient = new SmtpClient(_smtpMailSettings.Address, _smtpMailSettings.Port);
        try
        {
            var mailMessage = ConfigureSmtpClient(smtpClient);

            smtpClient.Send(mailMessage);
            return new MailResponse(true, 250, "Mail sent successfully");

        }
        catch (Exception exception)
        {
            return new MailResponse(false, 421, $"Sent failed! Error: {exception.Message}");
        }
    }

    /// <summary>
    /// This will send mail after initial setup in asynchronous way
    /// </summary>
    /// <returns>MailResponse with response code and message</returns>
    public async Task<MailResponse> SendAsync()
    {
        using var smtpClient = new SmtpClient(_smtpMailSettings.Address, _smtpMailSettings.Port);
        try
        {
            var mailMessage = ConfigureSmtpClient(smtpClient);

            await smtpClient.SendMailAsync(mailMessage);
            return new MailResponse(true, 250, "Mail sent successfully");

        }
        catch (Exception exception)
        {
            return new MailResponse(false, 421, $"Sent failed! Error: {exception.Message}");
        }
    }

    private MailMessage ConfigureSmtpClient(SmtpClient smtpClient)
    {
        smtpClient.UseDefaultCredentials = false;
        smtpClient.Credentials = new NetworkCredential(_smtpMailSettings.UserName, _smtpMailSettings.Password);
        smtpClient.EnableSsl = true;
        var message = new MailMessage
        {
            IsBodyHtml = _mailRequest.HtmlSupported,
            From = new MailAddress(_mailRequest.SenderAddress),
            Subject = _mailRequest.Subject,
            Body = _mailRequest.Body
        };
        _mailRequest.ToAddresses.ForEach(r => { message.To.Add(r); });
        _mailRequest.CcAddresses.ForEach(r => { message.CC.Add(r); });
        _mailRequest.BccAddresses.ForEach(r => { message.Bcc.Add(r); });
        
        return message;
    }
}