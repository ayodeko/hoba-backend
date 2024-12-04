using FirebaseAdmin.Auth;
using HobaBackend.Auth.Requests;

namespace HobaBackend.Auth;

public interface IAuthService
{
    void Init();
    Task<IUserInfo?> CreateUser(CreateAuthUser user);
    Task UpdateUser(UpdateAuthUser user);
    Task ChangePassword(string userId, string newPassword);
    Task SignInWithEmail(string email, string password);
    Task SignInWithUsername(string username, string password);
    Task<IUserInfo?> GetUserByEmail(string email);
}