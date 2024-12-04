namespace HobaBackend.Auth;

public class UpdateAuthUser
{
    public string username { get; set; }
    public string email { get; set; }
    public string firstName { get; set; }
    public string lastName { get; set; }
    public string password { get; set; }
    public string phoneNumber { get; set; }
}
public class CreateAuthUser : UpdateAuthUser
{
    public string email { get; set; }
}
public interface IAuthService
{
    void init();
    void createUser(CreateAuthUser user);
    void updateUser(UpdateAuthUser user);
    void changePassword(string userId, string newPassword);
    void signInWithEmail(string email, string password);
    void signInWithUsername(string username, string password);
}
