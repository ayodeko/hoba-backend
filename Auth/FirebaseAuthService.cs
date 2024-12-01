using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;

namespace Auth;

public class FirebaseAuthService : IAuthService
{
    public void init()
    {
        string CredentialEnvironmentVariable = "GOOGLE_APPLICATION_CREDENTIALS";
        var envariable = Environment.GetEnvironmentVariable(CredentialEnvironmentVariable);
        var gCred = GoogleCredential.GetApplicationDefault();
        FirebaseApp.Create(new AppOptions()
        {
            Credential = gCred,
            ProjectId = "hoba-backend",
        });
    }

    public async void createUser(CreateAuthUser user)
    {
        //Testing user creation
        UserRecordArgs args = new UserRecordArgs()
        {
            Email = user.email,
            EmailVerified = false,
            PhoneNumber = user.phoneNumber,
            Password = user.password,
            DisplayName = $"{user.firstName} {user.lastName}",
            Disabled = false,
        };
        UserRecord userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(args);
        // See the UserRecord reference doc for the contents of userRecord.
        Console.WriteLine($"Successfully created new user: {userRecord.Uid}");
    }

    public void updateUser(UpdateAuthUser user)
    {
        throw new NotImplementedException();
    }

    public void changePassword(string userId, string newPassword)
    {
        throw new NotImplementedException();
    }

    public void signInWithEmail(string email, string password)
    {
        throw new NotImplementedException();
    }

    public void signInWithUsername(string username, string password)
    {
        throw new NotImplementedException();
    }
}