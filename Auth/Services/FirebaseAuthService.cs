﻿using System.Net.Http.Json;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using HobaBackend.Auth.Firebase;
using HobaBackend.Auth.Options;
using HobaBackend.Auth.Requests;
using HobaBackend.Auth.Responses;
using HobaBackend.Auth.Utilities;
using HobaBackend.DB.Entities;
using HobaBackend.DB.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HobaBackend.Auth.Services;

public class FirebaseAuthService : IAuthService
{
    private readonly ILogger _logger;
    private readonly IPasswordGenerator _passwordGenerator;
    private readonly IUserRepository _userRepository;
    private readonly IEmailSender _emailSender;
    private readonly HttpClient _httpClient;
    private readonly FirebaseAuthConfig _firebaseAuthConfig;

    public FirebaseAuthService(
        ILogger<FirebaseAuthService> logger,
        IPasswordGenerator passwordGenerator,
        IUserRepository userRepository,
        IEmailSender emailSender,
        HttpClient httpClient,
        IOptions<FirebaseAuthConfig> firebaseAuthConfigOptions)
    {
        _logger = logger;
        _passwordGenerator = passwordGenerator;
        _userRepository = userRepository;
        _emailSender = emailSender;
        _httpClient = httpClient;
        _firebaseAuthConfig = firebaseAuthConfigOptions.Value;
        Init();
    }

    public void Init()
    {
        const string CredentialEnvironmentVariable = "HOBA_FIREBASE_CREDENTIALS_JSON";
        var jsonFirebaseCredential = Environment.GetEnvironmentVariable(CredentialEnvironmentVariable);
        var gCred = GoogleCredential.FromJson(jsonFirebaseCredential);
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

            await _emailSender.SendPasswordEmail(user.Email, generatedPassword);

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

    public async Task<SignInUserResponse> ChangePassword(string idToken, string newPassword, CancellationToken cancellationToken)
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