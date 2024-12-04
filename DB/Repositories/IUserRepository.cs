using HobaBackend.DB.Entities;

namespace HobaBackend.DB.Repositories;

public interface IUserRepository
{
    Task<HobaUser> CreateUser(HobaUser hobaUser, CancellationToken cancellationToken = default);
    Task<HobaUser?> GetUserByUsername(string username, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}