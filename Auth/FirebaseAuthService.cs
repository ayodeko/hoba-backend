using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using HobaBackend.Auth.Requests;
using HobaBackend.Auth.Utilities;
using Microsoft.Extensions.Logging;

namespace HobaBackend.Auth;

public class FirebaseAuthService : IAuthService
{
    private readonly ILogger _logger;
    private readonly IPasswordGenerator _passwordGenerator;

    public FirebaseAuthService(ILogger<FirebaseAuthService> logger, IPasswordGenerator passwordGenerator)
    {
        _logger = logger;
        _passwordGenerator = passwordGenerator;
    }

    public void Init()
    {
        string CredentialEnvironmentVariable = "GOOGLE_APPLICATION_CREDENTIALS";
        var envariable = Environment.GetEnvironmentVariable(CredentialEnvironmentVariable);
        var gCred = GoogleCredential.GetApplicationDefault();
        FirebaseApp.Create(new AppOptions
        {
            Credential = gCred,
            ProjectId = "hoba-backend",
        });
    }

    public async Task<IUserInfo?> CreateUser(CreateAuthUser user)
    {
        var generatedPassword = _passwordGenerator.Generate();

        UserRecordArgs args = new UserRecordArgs
        {
            Email = user.Email,
            EmailVerified = false,
            PhoneNumber = user.PhoneNumber,
            Password = generatedPassword,
            DisplayName = $"{user.FirstName} {user.LastName}",
            Disabled = false,
        };
        try
        {
            UserRecord? userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(args);

            return userRecord;
        }
        catch (FirebaseAuthException ex)
        {
            _logger.LogInformation(ex, "Failed to create user with email: {Email}", args.Email);
            return null;
        }
    }

    public Task UpdateUser(UpdateAuthUser user)
    {
        throw new NotImplementedException();
    }

    public Task ChangePassword(string userId, string newPassword)
    {
        throw new NotImplementedException();
    }

    public Task SignInWithEmail(string email, string password)
    {
        throw new NotImplementedException();
    }

    public Task SignInWithUsername(string username, string password)
    {
        throw new NotImplementedException();
    }

    public async Task<IUserInfo?> GetUserByEmail(string email)
    {
        try
        {
            var user = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(email);

            return user;
        }
        catch (FirebaseAuthException ex)
        {
            _logger.LogInformation(ex, "Failed to get user by email");
            return null;
        }
    }
}