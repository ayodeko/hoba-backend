namespace HobaBackend.Auth.Requests;

public class ChangePasswordRequest
{
    public required string IdToken { get; init; }
    public required string NewPassword { get; init; }
}