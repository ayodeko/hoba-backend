namespace HobaBackend.Auth.Responses;

public class SignInUserResponse
{
    public int Id { get; init; }
    public string Uid { get; init; }
    public string Username { get; init; }
    public string Email { get; init; }
    public string FullName { get; init; }
    public string PhoneNumber { get; init; }
    public string RefreshToken { get; init; }
    public string IDToken { get; init; }
    
    public string ExpiresIn { get; init; }
}