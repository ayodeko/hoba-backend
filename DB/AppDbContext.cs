using HobaBackend.DB.Entities;
using Microsoft.EntityFrameworkCore;

namespace HobaBackend.DB;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<HobaUser> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HobaUser>(entity =>
        {
            entity.Property(user => user.Id)
                .HasIdentityOptions(minValue: 10_000);
        });

        base.OnModelCreating(modelBuilder);
    }
}