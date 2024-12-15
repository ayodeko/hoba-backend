using HobaBackend.Auth.Requests;
using HobaBackend.Auth.Services;

namespace HobaBackend.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("users");

        MapCreateUser(group);
        MapSignInUser(group);
        MapGetByUsername(group);
        MapGetByUserId(group);
    }

    private static void MapCreateUser(RouteGroupBuilder group)
    {
        group.MapPost("/",
            async (CreateAuthUser createUserRequest, IAuthService authService, CancellationToken cancellationToken) =>
            {
                var user = await authService.CreateUser(createUserRequest, cancellationToken);

                return user is not null
                    ? Results.CreatedAtRoute("GetByUsername", new { user.Username }, user)
                    : Results.BadRequest($"Failed to create user with email: {createUserRequest.Email}");
            });
    }
    private static void MapSignInUser(RouteGroupBuilder group)
    {
        group.MapPost("/Login",
            async (SignInAuthUser signInAuthUser, IAuthService authService, CancellationToken cancellationToken) =>
            {
                var response = await authService.SignInWithEmail(signInAuthUser.Email, signInAuthUser.Password, cancellationToken);
                return response;
            });
        
    }

    private static void MapGetByUsername(RouteGroupBuilder group)
    {
        group.MapGet("{username}",
                async (string username, IAuthService authService, CancellationToken cancellationToken) =>
                {
                    var user = await authService.GetByUsername(username, cancellationToken);

                    return user is not null
                        ? Results.Ok(user)
                        : Results.NotFound($"{username} is not registered");
                })
            .WithName("GetByUsername");
    }

    private static void MapGetByUserId(RouteGroupBuilder group)
    {
        group.MapGet("{userId:int}",
                async (int userId, IAuthService authService, CancellationToken cancellationToken) =>
                {
                    var user = await authService.GetByUserId(userId, cancellationToken);

                    return user is not null
                        ? Results.Ok(user)
                        : Results.NotFound($"No one registered with id={userId}");
                })
            .WithName("GetByUserId");
    }
}