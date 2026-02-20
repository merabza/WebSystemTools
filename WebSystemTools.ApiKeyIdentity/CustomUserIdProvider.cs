using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace WebSystemTools.ApiKeyIdentity;

public sealed class CustomUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        // Implement user id acquiring logic  
        Claim? nameClaim = connection.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);
        return nameClaim?.Value;
    }
}
