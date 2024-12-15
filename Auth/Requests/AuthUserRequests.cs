namespace HobaBackend.Auth.Requests;

public class CreateAuthUser
{
    public string Username { get; init; }
    public string Email { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string PhoneNumber { get; init; }
}

public class SignInAuthUser
{
    public string Email { get; init; }
    public string Password { get; init; }
}