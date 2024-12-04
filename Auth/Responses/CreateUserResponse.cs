namespace HobaBackend.Auth.Responses;

public class CreateUserResponse
{
    public int Id { get; set; }
    public string Uid { get; set; }
    public string Username { get; init; }
    public string Email { get; init; }
    public string FullName { get; init; }
    public string PhoneNumber { get; set; }

    /// <summary>
    /// Temporary return this until emailing is setup
    /// </summary>
    public string GeneratedPassword { get; set; }
}