using System.Net;
using System.Net.Mail;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using HobaBackend.Auth.Options;
using HobaBackend.Auth.Services;
using HobaBackend.Auth.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HobaBackend.Auth;

public static class AuthServiceExtensions
{
    public static void AddAuthServices(this IServiceCollection services, IConfiguration configuration)
    {
        var jsonFirebaseCredential = Environment.GetEnvironmentVariable("HOBA_FIREBASE_CREDENTIALS_JSON");
        var gCred = GoogleCredential.FromJson(jsonFirebaseCredential);
        FirebaseApp.Create(new AppOptions
        {
            Credential = gCred,
            ProjectId = "hoba-backend",
        });

        services.AddScoped<IAuthService, FirebaseAuthService>();
        services.AddTransient<IPasswordGenerator, PasswordGenerator>();
        services.AddScoped<IEmailSender, EmailSender>();
        services.ConfigureOptions<ConfigureFirebaseAuth>();

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