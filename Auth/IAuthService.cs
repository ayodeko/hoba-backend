using HobaBackend.Auth.Requests;
using HobaBackend.Auth.Responses;

namespace HobaBackend.Auth;

public interface IAuthService
{
    void Init();
    Task<CreateUserResponse?> CreateUser(CreateAuthUser user, CancellationToken cancellationToken = default);
    Task UpdateUser(UpdateAuthUser user);
    Task ChangePassword(string userId, string newPassword);
    Task SignInWithEmail(string email, string password);
    Task SignInWithUsername(string username, string password);
    Task<GetUserResponse?> GetUserByUsername(string username, CancellationToken cancellationToken = default);
}