using HobaBackend.Auth.Messaging;
using HobaBackend.DB;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace HobaBackend;

public static class Extensions
{
    public static async Task ApplyMigrations(this WebApplication app)
    {
        var serviceProvider = app.Services
            .CreateScope()
            .ServiceProvider;

        var authContext = serviceProvider.GetRequiredService<AppDbContext>();
        var authMigrations = await authContext.Database.GetPendingMigrationsAsync();

        if (authMigrations.Any())
        {
            await authContext.Database.MigrateAsync();
        }
    }

    public static void ConfigureMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<SqlTransportOptions>()
            .Configure(options =>
            {
                options.ConnectionString = configuration.GetConnectionString("NeonConnection");
                options.Schema = "message-broker";
                options.AdminUsername = configuration["ConnectionStrings:AdminUsername"];
                options.AdminPassword = configuration["ConnectionStrings:AdminPassword"];
                options.Role = configuration["ConnectionStrings:AdminUsername"];
            });

        services.AddPostgresMigrationHostedService(options => { options.CreateDatabase = false; });

        services.AddMassTransit(bus =>
        {
            bus.SetKebabCaseEndpointNameFormatter();

            bus.AddSqlMessageScheduler();

            bus.AddConsumer<UserCreatedMessageConsumer>();

            bus.UsingPostgres((context, configurator) =>
            {
                configurator.UseSqlMessageScheduler();
                configurator.AutoStart = true;
                configurator.ConfigureEndpoints(context);
            });
        });
    }
}