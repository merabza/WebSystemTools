using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace ApiKeyIdentity;

public class CustomUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        // Implement user id acquiring logic  
        var nameClaim = connection.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);
        return nameClaim?.Value;
    }
}