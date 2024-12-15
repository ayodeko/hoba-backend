using HobaBackend.Auth.Requests;
using HobaBackend.Auth.Responses;

namespace HobaBackend.Auth;

public interface IAuthService
{
    void Init();
    Task<CreateUserResponse?> CreateUser(CreateAuthUser user, CancellationToken cancellationToken = default);
    Task UpdateUser(UpdateAuthUser user);
    Task ChangePassword(string userId, string newPassword);
    Task<SignInUserResponse> SignInWithEmail(string email, string password, CancellationToken cancellationToken = default);
    Task SignInWithUsername(string username, string password);
    Task<GetUserResponse?> GetByUsername(string username, CancellationToken cancellationToken = default);
    Task<GetUserResponse?> GetByUserId(int userId, CancellationToken cancellationToken = default);
}