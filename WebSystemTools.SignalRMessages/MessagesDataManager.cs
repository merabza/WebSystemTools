using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SystemTools.StringMessagesApiContracts;
using SystemTools.SystemToolsShared;

namespace WebSystemTools.SignalRMessages;

public sealed class MessagesDataManager : IMessagesDataManager
{
    private readonly Dictionary<string, List<string>> _connectedUsers = new();
    private readonly IHubContext<MessagesHub, IMessenger> _hub;

    // ReSharper disable once ConvertToPrimaryConstructor
    public MessagesDataManager(IHubContext<MessagesHub, IMessenger> hub)
    {
        _hub = hub;
    }

    public async Task SendMessage(string? userName, string message, CancellationToken cancellationToken = default)
    {
        if (userName is null)
        {
            return;
        }

        if (!_connectedUsers.TryGetValue(userName, out List<string>? conList))
        {
            return;
        }

        //_logger.LogInformation("Try to send message: {message}", message);
        foreach (string connectionId in conList)
        {
            await _hub.Clients.Client(connectionId).SendMessage(message, cancellationToken);
        }
        //ესეც მუშაობს, უბრალოდ მომხმარებლიდ არ არის გაფილტრული
        //await _hub.Clients.All.SendMessage(message, cancellationToken);
    }

    public void UserConnected(string connectionId, string userName)
    {
        if (!_connectedUsers.TryGetValue(userName, out List<string>? conList))
        {
            conList = [];
            _connectedUsers.Add(userName, conList);
        }

        if (!conList.Contains(connectionId))
        {
            conList.Add(connectionId);
        }
    }

    public void UserDisconnected(string connectionId, string userName)
    {
        if (!_connectedUsers.TryGetValue(userName, out List<string>? conList))
        {
            return;
        }

        if (!conList.Contains(connectionId))
        {
            return;
        }

        conList.Remove(connectionId);
        if (conList.Count == 0)
        {
            _connectedUsers.Remove(userName);
        }
    }
}
