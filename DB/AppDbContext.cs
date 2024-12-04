using Microsoft.EntityFrameworkCore;

namespace HobaBackend.DB;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}