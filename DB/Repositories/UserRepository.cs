using HobaBackend.DB.Entities;
using Microsoft.EntityFrameworkCore;

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

    public async Task<HobaUser?> GetUserByUsername(string username, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(user => user.Username == username)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<HobaUser?> GetUserByUserId(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(user => user.Id == userId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}