using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace HobaBackend.Auth.Options;

public class ConfigureFirebaseAuth : IConfigureOptions<FirebaseAuthConfig>
{
    private readonly IConfiguration _configuration;

    public ConfigureFirebaseAuth(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(FirebaseAuthConfig options)
    {
        _configuration.GetSection(nameof(FirebaseAuthConfig)).Bind(options);
    }
}