namespace HobaBackend.Auth.Firebase;

public class FirebaseChangePasswordRestResponse
{
    public string localId { get; set; }
    public string email { get; set; }
    public string passwordHash { get; set; }
    public string idToken { get; set; }
    public string refreshToken { get; set; }
    public string expiresIn { get; set; }
}
