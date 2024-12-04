namespace HobaBackend.Auth.Responses;

public class GetUserResponse
{
    public int Id { get; set; }
    public string Uid { get; set; }
    public string Username { get; init; }
    public string Email { get; init; }
    public string FullName { get; init; }
    public string PhoneNumber { get; set; }
}