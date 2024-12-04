using FirebaseAdmin.Auth;
using HobaBackend.Auth.Requests;
using HobaBackend.Auth.Responses;

namespace HobaBackend.Auth;

public interface IAuthService
{
    void Init();
    Task<CreateUserResponse?> CreateUser(CreateAuthUser user, CancellationToken cancellationToken);
    Task UpdateUser(UpdateAuthUser user);
    Task ChangePassword(string userId, string newPassword);
    Task SignInWithEmail(string email, string password);
    Task SignInWithUsername(string username, string password);
    Task<IUserInfo?> GetUserByEmail(string email);
}