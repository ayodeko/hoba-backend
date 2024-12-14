using System.Net;
using System.Net.Mail;
using HobaBackend.Auth.Services;
using HobaBackend.Auth.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HobaBackend.Auth;

public static class AuthServiceExtensions
{
    public static void AddAuthServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IAuthService, FirebaseAuthService>();
        services.AddSingleton<IPasswordGenerator, PasswordGenerator>();
        services.AddSingleton<IEmailSender, EmailSender>();

        services.AddFluentEmail(configuration["EmailCred:SenderEmail"], configuration["EmailCred:DefaultName"])
            .AddRazorRenderer()
            .AddSmtpSender(new SmtpClient(configuration.GetValue<string>("EmailCred:Host"),
                configuration.GetValue<int>("EmailCred:Port"))
            {
                Credentials = new NetworkCredential(configuration["EmailCred:SenderEmail"],
                    configuration["EmailCred:Password"]),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network
            });
    }
}