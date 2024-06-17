using Microsoft.AspNetCore.Authentication;

namespace SignalRMessages;

public static class DependencyInjectionExtensions
{
    public static AuthenticationBuilder AddApiKeyAuthenticationSchema(this AuthenticationBuilder authentication)
    {
        authentication.AddScheme<AuthenticationSchemeOptions, TokenAuthenticationHandler>(
            AuthenticationSchemaNames.ApiKeyAuthentication, _ => { });
        return authentication;
    }
}