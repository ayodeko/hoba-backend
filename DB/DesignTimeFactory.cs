using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace HobaBackend.DB;

/// <summary>
/// Used to generate migrations in different assembly other than the entry (api project) assembly 
/// </summary>
public class DesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    private readonly IConfiguration? _configuration;

    public DesignTimeFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public DesignTimeFactory()
    {
        // Required for this to work
    }

    public AppDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<AppDbContext>();

        builder.UseNpgsql(_configuration?.GetConnectionString("DbConnection"));

        return new AppDbContext(builder.Options);
    }
}