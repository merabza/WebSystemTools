using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SignalRContracts;

namespace SignalRMessages;

public class MessagesDataManager : IMessagesDataManager
{
    private readonly Dictionary<string, List<string>> _connectedUsers = [];
    private readonly IHubContext<MessagesHub, IMessenger> _hub;
    private readonly ILogger<MessagesDataManager> _logger;

    // ReSharper disable once ConvertToPrimaryConstructor
    public MessagesDataManager(IHubContext<MessagesHub, IMessenger> hub, ILogger<MessagesDataManager> logger)
    {
        _hub = hub;
        _logger = logger;
    }

    public async Task SendMessage(string? userName, string message, CancellationToken cancellationToken)
    {
        if (userName is null)
            return;
        if (!_connectedUsers.TryGetValue(userName, out var conList))
            return;

        _logger.LogInformation("Try to send message: {message}", message);
        foreach (var connectionId in conList)
            await _hub.Clients.Client(connectionId).SendMessage(message, cancellationToken);
        await _hub.Clients.All.SendMessage(message, cancellationToken);
    }

    public void UserConnected(string connectionId, string userName)
    {
        if (!_connectedUsers.ContainsKey(userName))
            _connectedUsers.Add(userName, []);
        var conList = _connectedUsers[userName];
        if (!conList.Contains(connectionId))
            conList.Add(connectionId);
    }

    public void UserDisconnected(string connectionId, string userName)
    {
        if (!_connectedUsers.TryGetValue(userName, out var conList))
            return;
        if (!conList.Contains(connectionId))
            return;
        conList.Remove(connectionId);
        if (conList.Count == 0)
            _connectedUsers.Remove(userName);
    }
}