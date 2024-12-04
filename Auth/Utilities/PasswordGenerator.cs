using System.Text;

namespace HobaBackend.Auth.Utilities;

public class PasswordGenerator : IPasswordGenerator
{
    private const int PasswordLength = 10;
    private const string Alphanumeric = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    public string Generate()
    {
        var passwordBuilder = new StringBuilder();

        for (var i = 0; i < PasswordLength; i++)
        {
            var letter = Alphanumeric[Random.Shared.Next(Alphanumeric.Length)];
            passwordBuilder.Append(letter);
        }

        return passwordBuilder.ToString();
    }
}