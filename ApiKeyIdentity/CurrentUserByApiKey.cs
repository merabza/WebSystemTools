using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ApiKeyIdentity;

public sealed class CurrentUserByApiKey : ICurrentUserByApiKey
{
    private readonly IHttpContextAccessor _httpContext;

    // ReSharper disable once ConvertToPrimaryConstructor
    public CurrentUserByApiKey(IHttpContextAccessor httpContext)
    {
        _httpContext = httpContext;
    }

    public string Name => GetClaimValue<string>(ClaimTypes.Name);

    private T GetClaimValue<T>(string type) where T : IConvertible
    {
        var value = _httpContext.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == type)?.Value;

        return value != null
            ? (T)Convert.ChangeType(value, typeof(T))
            : throw new UnauthorizedAccessException($"{type} claim not found");
    }
}