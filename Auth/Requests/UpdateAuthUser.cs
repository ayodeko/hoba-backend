namespace HobaBackend.Auth.Requests;

public class UpdateAuthUser
{
    public string Username { get; init; }
    public string Email { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string PhoneNumber { get; init; }
}