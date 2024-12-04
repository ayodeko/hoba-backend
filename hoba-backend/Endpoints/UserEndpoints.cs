using HobaBackend.Auth;
using HobaBackend.Auth.Requests;

namespace HobaBackend.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("users");

        MapCreateUser(group);
        MapGetUserByEmail(group);
    }

    private static void MapCreateUser(RouteGroupBuilder group)
    {
        group.MapPost("/",
            async (CreateAuthUser createUserRequest, IAuthService authService, CancellationToken cancellationToken) =>
            {
                var user = await authService.CreateUser(createUserRequest, cancellationToken);

                return user is not null
                    ? Results.CreatedAtRoute("GetByEmail", new { user.Email }, user)
                    : Results.BadRequest($"Failed to create user with email: {createUserRequest.Email}");
            });
    }

    private static void MapGetUserByEmail(RouteGroupBuilder group)
    {
        group.MapGet("{email}", async (string email, IAuthService authService, CancellationToken cancellationToken) =>
            {
                var user = await authService.GetUserByEmail(email);

                return user is not null
                    ? Results.Ok(user)
                    : Results.NotFound($"{email} is not registered");
            })
            .WithName("GetByEmail");
    }
}