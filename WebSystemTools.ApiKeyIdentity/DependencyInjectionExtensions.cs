using Microsoft.AspNetCore.Authentication;

namespace WebSystemTools.ApiKeyIdentity;

public static class DependencyInjectionExtensions
{
    public static AuthenticationBuilder AddApiKeyAuthenticationSchema(this AuthenticationBuilder authentication)
    {
        authentication.AddScheme<AuthenticationSchemeOptions, TokenAuthenticationHandler>(
            AuthenticationSchemaNames.ApiKeyAuthentication, _ => { });
        return authentication;
    }
}
