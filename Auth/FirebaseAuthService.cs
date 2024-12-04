using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using HobaBackend.Auth.Requests;
using HobaBackend.Auth.Responses;
using HobaBackend.Auth.Utilities;
using HobaBackend.DB.Entities;
using HobaBackend.DB.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HobaBackend.Auth;

public class FirebaseAuthService : IAuthService
{
    private readonly ILogger _logger;
    private readonly IPasswordGenerator _passwordGenerator;
    private readonly IUserRepository _userRepository;

    public FirebaseAuthService(
        ILogger<FirebaseAuthService> logger,
        IPasswordGenerator passwordGenerator,
        IUserRepository userRepository)
    {
        _logger = logger;
        _passwordGenerator = passwordGenerator;
        _userRepository = userRepository;
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

    public async Task<CreateUserResponse?> CreateUser(CreateAuthUser user, CancellationToken cancellationToken)
    {
        var generatedPassword = _passwordGenerator.Generate();

        var args = new UserRecordArgs
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
            var firebaseUser = await FirebaseAuth.DefaultInstance.CreateUserAsync(args, cancellationToken);

            var hobaUser = new HobaUser
            {
                Uid = firebaseUser.Uid,
                Username = user.Username
            };

            var createdHobaUser = await _userRepository.CreateUser(hobaUser, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

            // Send email immediately or using events

            return new CreateUserResponse
            {
                Username = createdHobaUser.Username,
                Email = firebaseUser.Email,
                FullName = firebaseUser.DisplayName,
                PhoneNumber = firebaseUser.PhoneNumber,
                Id = createdHobaUser.Id,
                Uid = createdHobaUser.Uid,
                GeneratedPassword = generatedPassword
            };
        }
        catch (FirebaseAuthException ex)
        {
            _logger.LogInformation(ex, "Failed to create user with email: {Email}", args.Email);
            return default;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogInformation(ex, "Failed to create user with email: {Email}", args.Email);
            return default;
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

    public async Task<GetUserResponse?> GetByUsername(string username,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var customUser = await _userRepository.GetUserByUsername(username, cancellationToken);

            if (customUser is null)
            {
                return default;
            }

            var firebaseUser = await FirebaseAuth.DefaultInstance.GetUserAsync(customUser.Uid, cancellationToken);

            return new GetUserResponse
            {
                Email = firebaseUser.Email,
                FullName = firebaseUser.DisplayName,
                Id = customUser.Id,
                PhoneNumber = firebaseUser.PhoneNumber,
                Uid = firebaseUser.Uid,
                Username = customUser.Username
            };
        }
        catch (FirebaseAuthException ex)
        {
            _logger.LogInformation(ex, "Failed to get user by username");
            return default;
        }
    }

    public async Task<GetUserResponse?> GetByUserId(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var customUser = await _userRepository.GetUserByUserId(userId, cancellationToken);

            if (customUser is null)
            {
                return default;
            }

            var firebaseUser = await FirebaseAuth.DefaultInstance.GetUserAsync(customUser.Uid, cancellationToken);

            return new GetUserResponse
            {
                Email = firebaseUser.Email,
                FullName = firebaseUser.DisplayName,
                Id = customUser.Id,
                PhoneNumber = firebaseUser.PhoneNumber,
                Uid = firebaseUser.Uid,
                Username = customUser.Username
            };
        }
        catch (FirebaseAuthException ex)
        {
            _logger.LogInformation(ex, "Failed to get user by username");
            return default;
        }
    }
}