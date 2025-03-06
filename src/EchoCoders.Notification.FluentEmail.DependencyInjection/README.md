# EchoCoders.Notification.FluentEmail

## A Email Sending Package which supports both SMTP and Azure Communication Mail service. It supports Fluent API format to configure and send email.

### Available Fluent Methods

` Setup() `
This method will be used to setup related mail settings such as sender address, TO, CC, BCC, HTML supports etc.

`FormatSubjet()`
With the help of this method, we can format the subject. It supports dynamic parameter placing with pattern matching.
For an example, you have a configured subject as: "OTP for ##REASON##", then you can replace this ##REASON## with any of your dynamic parameter value with this method. The calling procedure is as follows:

```
var paramDictionary = new Dictionary<string, string>();

paramDictionary.Add("REASON", "Password Change");

.FormatSubject("OTP for ##REASON##", "##", paramDictionary)

```
After calling this method, the subject will transform to: "OTP for Password Change"

`FormatBody()`
With the help of this method, we can format the body. It supports dynamic parameter placing with pattern matching same as `FormatSubject()`.
For an example, you have a configured body as: <p>Dear ##NAME##,</p><p>Your OTP is ##OTP##</p>, then you can replace this ##OTP## with any of your dynamic parameter value with this method. The calling procedure is as follows:

```
var paramDictionary = new Dictionary<string, string>();

paramDictionary.Add("OTP", "123456");

paramDictionary.Add("NAME", "Mr. X");

.FormatBody("<p>Dear ##NAME##,</p><p>Your OTP is ##OTP##</p>", "##", paramDictionary)

```
After calling this method, the subject will transform to: <p>Dear Mr. X,</p><p>Your OTP is 123456</p>

`SendMail()` and `SendMailAsync()` two methods to send mail synchronuous or asynchronuous call.

## Full Code to send mail via Azure Communication Mail

```
IFluentMailService mailService = new FluentAzureMailService(new AzureMailSettings(ConnectionString:"you_azure_mail_connection_string"));

var paramDictionary = new Dictionary<string, string>();
paramDictionary.Add("OTP", "123456");
paramDictionary.Add("NAME", "Mr. X");
paramDictionary.Add("REASON", "Password Change");

await mailService
    .Setup(
        "info@your_mail.com", //sender
        new List<string>{"sender@your_mail.com"}, //TO list
        new List<string>(), //CC list
        new List<string>(), //BCC list
        true // HTML Support or not
    )
    .FormatSubject("OTP for ##REASON##", "##", paramDictionary)
    .FormatBody("<p>Dear ##NAME##,</p><p>Your OTP is ##OTP##</p>", "##", paramDictionary)
    .SendAsync();
```

## Full Code to send mail via SMTP

```
IFluentMailService mailService = new FluentSmtpMailService(new SmtpMailSettings("smtp_address", 587, "smtp_sender@mail.com","your_smtp_password"));

var paramDictionary = new Dictionary<string, string>();
paramDictionary.Add("OTP", "123456");
paramDictionary.Add("NAME", "Mr. X");
paramDictionary.Add("REASON", "Password Change");

await mailService
    .Setup(
        "info@your_mail.com", //sender
        new List<string>{"sender@your_mail.com"}, //TO list
        new List<string>(), //CC list
        new List<string>(), //BCC list
        true // HTML Support or not
    )
    .FormatSubject("OTP for ##REASON##", "##", paramDictionary)
    .FormatBody("<p>Dear ##NAME##,</p><p>Your OTP is ##OTP##</p>", "##", paramDictionary)
    .SendAsync();
```

## EchoCoders.Notification.FluentEmail.DependencyInjection
There is a supplementary package which covers dependency injection in .NET Core framework. To inject the depdency we need to call following methods:

```
services.AddFluentAzureEmailService(new AzureMailSettings("your_connection_string"));
services.AddFluentSmtpEmailService(new SmtpMailSettings("your_smtp_address", 587, "smtp_user_name", "smtp_password"));
```

## Supported .NET versions
.NET 8 and upper