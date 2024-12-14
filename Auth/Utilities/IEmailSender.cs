namespace HobaBackend.Auth.Utilities;

public interface IEmailSender
{
    public Task SendPasswordEmail(string recipientEmail, string generatedPassword);
}