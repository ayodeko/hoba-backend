using HobaBackend.Auth;

namespace HobaBackend;

public static class Extensions
{
    public static void UseCustomAuth(this WebApplication app)
    {
        var auth = app.Services.GetService<IAuthService>();
        auth.init();
    }
}