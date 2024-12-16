namespace HobaBackend.Auth.Messaging;

public record UserCreatedMessage(
    int Id,
    string Uid,
    string Username,
    string Email,
    string GeneratedPassword
);