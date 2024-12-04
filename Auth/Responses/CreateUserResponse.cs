namespace HobaBackend.Auth.Responses;

public class CreateUserResponse
{
    public int Id { get; init; }
    public string Uid { get; init; }
    public string Username { get; init; }
    public string Email { get; init; }
    public string FullName { get; init; }
    public string PhoneNumber { get; init; }

    /// <summary>
    /// Temporary return this until emailing is setup
    /// </summary>
    public string GeneratedPassword { get; init; }
}