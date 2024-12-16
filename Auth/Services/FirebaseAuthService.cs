using System.Net.Http.Json;
using FirebaseAdmin.Auth;
using HobaBackend.Auth.Firebase;
using HobaBackend.Auth.Messaging;
using HobaBackend.Auth.Options;
using HobaBackend.Auth.Requests;
using HobaBackend.Auth.Responses;
using HobaBackend.Auth.Utilities;
using HobaBackend.DB.Entities;
using HobaBackend.DB.Repositories;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HobaBackend.Auth.Services;

public class FirebaseAuthService : IAuthService
{
    private readonly ILogger _logger;
    private readonly IPasswordGenerator _passwordGenerator;
    private readonly IUserRepository _userRepository;
    private readonly HttpClient _httpClient;
    private readonly FirebaseAuthConfig _firebaseAuthConfig;
    private readonly IPublishEndpoint _publishEndpoint;

    public FirebaseAuthService(
        ILogger<FirebaseAuthService> logger,
        IPasswordGenerator passwordGenerator,
        IUserRepository userRepository,
        HttpClient httpClient,
        IOptions<FirebaseAuthConfig> firebaseAuthConfigOptions,
        IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _passwordGenerator = passwordGenerator;
        _userRepository = userRepository;
        _httpClient = httpClient;
        _publishEndpoint = publishEndpoint;
        _firebaseAuthConfig = firebaseAuthConfigOptions.Value;
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

            await _publishEndpoint.Publish(new UserCreatedMessage(
                    createdHobaUser.Id,
                    firebaseUser.Uid,
                    createdHobaUser.Username,
                    firebaseUser.Email,
                    generatedPassword
                ),
                cancellationToken
            );

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

    public async Task<SignInUserResponse> ChangePassword(string idToken, string newPassword,
        CancellationToken cancellationToken)
    {
        var url = $"{_firebaseAuthConfig.ChangePasswordUrl}?key={_firebaseAuthConfig.ApiKey}";

        var payload = new
        {
            idToken,
            password = newPassword,
            returnSecureToken = true
        };

        var response = await _httpClient.PostAsJsonAsync(url, payload, cancellationToken);
        response.EnsureSuccessStatusCode();

        var firebaseResponse =
            await response
                .Content
                .ReadFromJsonAsync<FirebaseChangePasswordRestResponse>(cancellationToken);

        return new SignInUserResponse
        {
            RefreshToken = firebaseResponse.refreshToken,
            IDToken = firebaseResponse.idToken,
            Uid = firebaseResponse.localId,
            ExpiresIn = firebaseResponse.expiresIn
        };
    }

    public async Task<SignInUserResponse> SignInWithEmail(string email, string password,
        CancellationToken cancellationToken)
    {
        var url = $"{_firebaseAuthConfig.SignInUrl}?key={_firebaseAuthConfig.ApiKey}";
        var payload = new
        {
            email,
            password,
            returnSecureToken = true
        };

        var httpResponse = await _httpClient.PostAsJsonAsync(url, payload, cancellationToken);
        httpResponse.EnsureSuccessStatusCode();

        var firebaseResponse = await httpResponse
            .Content
            .ReadFromJsonAsync<FirebaseSignInUserRestResponse>(cancellationToken);

        return new SignInUserResponse
        {
            RefreshToken = firebaseResponse.refreshToken,
            IDToken = firebaseResponse.idToken,
            Uid = firebaseResponse.localId,
            FullName = firebaseResponse.displayName,
            ExpiresIn = firebaseResponse.expiresIn
        };
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