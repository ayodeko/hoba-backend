using HobaBackend.DB.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HobaBackend.DB;

public static class DbServiceExtensions
{
    public static void AddDbServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddDbContext<AppDbContext>(opt => 
                opt.UseNpgsql(configuration.GetConnectionString("NeonConnection")),
            contextLifetime: ServiceLifetime.Singleton);
    }
}