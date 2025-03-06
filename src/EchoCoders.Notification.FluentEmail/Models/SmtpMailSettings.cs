namespace EchoCoders.Notification.FluentEmail.Models;

public record SmtpMailSettings(string Address, int Port, string UserName, string Password);