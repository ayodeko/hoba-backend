using HobaBackend.Auth;
using HobaBackend.Auth.Requests;

namespace HobaBackend.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("users");

        MapCreateUser(group);
        MapGetUserByUsername(group);
    }

    private static void MapCreateUser(RouteGroupBuilder group)
    {
        group.MapPost("/",
            async (CreateAuthUser createUserRequest, IAuthService authService, CancellationToken cancellationToken) =>
            {
                var user = await authService.CreateUser(createUserRequest, cancellationToken);

                return user is not null
                    ? Results.CreatedAtRoute("GetUserByUsername", new { user.Email }, user)
                    : Results.BadRequest($"Failed to create user with email: {createUserRequest.Email}");
            });
    }

    private static void MapGetUserByUsername(RouteGroupBuilder group)
    {
        group.MapGet("{username}",
                async (string username, IAuthService authService, CancellationToken cancellationToken) =>
                {
                    var user = await authService.GetUserByUsername(username, cancellationToken);

                    return user is not null
                        ? Results.Ok(user)
                        : Results.NotFound($"{username} is not registered");
                })
            .WithName("GetUserByUsername");
    }
}