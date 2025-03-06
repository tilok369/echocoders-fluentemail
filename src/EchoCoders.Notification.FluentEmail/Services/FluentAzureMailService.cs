using Azure;
using Azure.Communication.Email;
using EchoCoders.Notification.FluentEmail.Contracts;
using EchoCoders.Notification.FluentEmail.Helper;
using EchoCoders.Notification.FluentEmail.Models;

namespace EchoCoders.Notification.FluentEmail.Services;

public class FluentAzureMailService: IFluentMailService
{
    private readonly AzureMailSettings _azureMailSettings;
    private MailRequest _mailRequest;
    
    public FluentAzureMailService(AzureMailSettings azureMailSettings)
    {
        _azureMailSettings = azureMailSettings;
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
    public IFluentMailService Setup(string senderAddress, List<string> toAddresses, List<string> ccAddresses, List<string> bccAddresses,
        bool htmlSupported = true)
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
        var emailClient = new EmailClient(_azureMailSettings.ConnectionString);
        var toRecipients = _mailRequest.ToAddresses.Any() ? _mailRequest.ToAddresses.Select(t =>
            new EmailAddress($"<{t}>")) : null;
        var ccRecipients = _mailRequest.CcAddresses.Any() ? _mailRequest.CcAddresses.Select(t =>
            new EmailAddress($"<{t}>")) : null;
        var bccRecipients = _mailRequest.BccAddresses.Any() ? _mailRequest.BccAddresses.Select(t =>
            new EmailAddress($"<{t}>")) : null;

        var emailContent = new EmailContent(_mailRequest.Subject);
        if (_mailRequest.HtmlSupported)
            emailContent.Html = _mailRequest.Body;
        else 
            emailContent.PlainText = _mailRequest.Body;

        var allEmailRecipients = new EmailRecipients(toRecipients, ccRecipients, bccRecipients);

        var emailMessage = new EmailMessage(
            senderAddress: _mailRequest.SenderAddress,
            allEmailRecipients,
            emailContent);

        try
        {
            var emailSendOperation = emailClient.Send(WaitUntil.Completed, emailMessage);

            return new MailResponse(true, long.Parse(emailSendOperation.Id.ToString()), $"Email Sent Successfully. Status = {emailSendOperation.Value.Status}");
        }
        catch (RequestFailedException ex)
        {
            return new MailResponse(true, long.Parse(ex.ErrorCode?.ToString() ?? "0"), $"Error while sending mail. Error = {ex.Message}");
        }
    }

    
    /// <summary>
    /// This will send mail after initial setup in asynchronous way
    /// </summary>
    /// <returns>MailResponse with response code and message</returns>
    public async Task<MailResponse> SendAsync()
    {
        var emailClient = new EmailClient(_azureMailSettings.ConnectionString);
        var toRecipients = _mailRequest.ToAddresses.Any() ? _mailRequest.ToAddresses.Select(t =>
            new EmailAddress($"<{t}>")) : null;
        var ccRecipients = _mailRequest.CcAddresses.Any() ? _mailRequest.CcAddresses.Select(t =>
            new EmailAddress($"<{t}>")) : null;
        var bccRecipients = _mailRequest.BccAddresses.Any() ? _mailRequest.BccAddresses.Select(t =>
            new EmailAddress($"<{t}>")) : null;

        var emailContent = new EmailContent(_mailRequest.Subject);
        if (_mailRequest.HtmlSupported)
            emailContent.Html = _mailRequest.Body;
        else emailContent.PlainText = _mailRequest.Body;

        var allEmailRecipients = new EmailRecipients(toRecipients, ccRecipients, bccRecipients);

        var emailMessage = new EmailMessage(
            senderAddress: _mailRequest.SenderAddress,
            allEmailRecipients,
            emailContent);

        try
        {
            var emailSendOperation = await emailClient.SendAsync(WaitUntil.Completed, emailMessage);

            return new MailResponse(true, 0, $"Email Sent Successfully. Status = {emailSendOperation.Value.Status}");
        }
        catch (RequestFailedException ex)
        {
            return new MailResponse(true, 0, $"Email Sent Successfully. Status = {ex.Message}");
        }
    }
}