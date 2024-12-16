using HobaBackend.Auth.Utilities;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace HobaBackend.Auth.Messaging;

public class UserCreatedMessageConsumer : IConsumer<UserCreatedMessage>
{
    private readonly IEmailSender _emailSender;
    private readonly ILogger<UserCreatedMessageConsumer> _logger;

    public UserCreatedMessageConsumer(IEmailSender emailSender, ILogger<UserCreatedMessageConsumer> logger)
    {
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UserCreatedMessage> context)
    {
        _logger.LogInformation("Started Processing {Message}", nameof(UserCreatedMessage));

        try
        {
            await _emailSender.SendPasswordEmail(context.Message.Email, context.Message.GeneratedPassword);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error Processing {Message}. Exception Message: {ExceptionMessage}",
                nameof(UserCreatedMessage), ex.Message
            );
        }

        _logger.LogInformation("Finished Processing {Message}", nameof(UserCreatedMessage));
    }
}