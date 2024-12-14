using FluentEmail.Core;

namespace HobaBackend.Auth.Utilities;

public class EmailSender : IEmailSender
{
    private readonly IFluentEmail _fluentEmail;

    public EmailSender(IFluentEmail fluentEmail)
    {
        _fluentEmail = fluentEmail;
    }
    
    public async Task SendPasswordEmail(string recipientEmail, string generatedPassword)
    {
        // TODO: Need to create a better template for this password email message
        await _fluentEmail
            .To(recipientEmail)
            .Subject("Account Successfully Created")
            .Body($"<div>Your password is <b>{generatedPassword}</b></div>", isHtml: true)
            .SendAsync();
    }
}