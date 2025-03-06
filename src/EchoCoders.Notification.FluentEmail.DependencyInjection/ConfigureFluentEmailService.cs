using EchoCoders.Notification.FluentEmail.Contracts;
using EchoCoders.Notification.FluentEmail.Models;
using EchoCoders.Notification.FluentEmail.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EchoCoders.Notification.FluentEmail.DependencyInjection;

public static class ConfigureFluentEmailService
{
    public static IServiceCollection AddFluentSmtpEmailService(
        this IServiceCollection services,
        SmtpMailSettings smtpMailSettings)
    {
        services.AddScoped<IFluentMailService>(s => new FluentSmtpMailService(smtpMailSettings));
        return services;
    }
    
    public static IServiceCollection AddFluentAzureEmailService(
        this IServiceCollection services,
        AzureMailSettings azureMailSettings)
    {
        services.AddScoped<IFluentMailService>(s => new FluentAzureMailService(azureMailSettings));
        return services;
    }
}