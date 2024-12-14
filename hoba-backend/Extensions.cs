using HobaBackend.Auth.Services;
using HobaBackend.DB;
using Microsoft.EntityFrameworkCore;

namespace HobaBackend;

public static class Extensions
{
    public static void UseCustomAuth(this WebApplication app)
    {
        var auth = app.Services.GetRequiredService<IAuthService>();
        auth.Init();
    }

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
}