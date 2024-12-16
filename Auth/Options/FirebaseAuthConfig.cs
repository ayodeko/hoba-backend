namespace HobaBackend.Auth.Options;

public class FirebaseAuthConfig
{
    public required string ApiKey { get; init; }
    public required string SignInUrl { get; init; }
    public required string ChangePasswordUrl { get; init; }
}