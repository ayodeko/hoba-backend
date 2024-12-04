using HobaBackend.DB.Entities;

namespace HobaBackend.DB.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<HobaUser> CreateUser(HobaUser hobaUser, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Users.AddAsync(hobaUser, cancellationToken);

        return entry.Entity;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}