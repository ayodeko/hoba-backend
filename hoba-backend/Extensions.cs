using HobaBackend.Auth;

namespace HobaBackend;

public static class Extensions
{
    public static void UseCustomAuth(this WebApplication app)
    {
        var auth = app.Services.GetRequiredService<IAuthService>();
        auth.Init();
    }
}