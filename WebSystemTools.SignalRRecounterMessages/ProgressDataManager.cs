using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SystemTools.ReCounterAbstraction;
using SystemTools.ReCounterContracts;

namespace WebSystemTools.SignalRRecounterMessages;

public sealed class ProgressDataManager : IProgressDataManager, IDisposable, IAsyncDisposable
{
    private static readonly Lock SyncRoot = new();
    private readonly Dictionary<string, List<string>> _connectedUsers = [];
    private readonly IHubContext<ReCounterMessagesHub, IProgressDataMessenger> _hub;
    private readonly ILogger<ProgressDataManager> _logger;
    private int _currentChangeId;
    private int _sentChangeId;

    private Timer? _timer;
    private bool _timerStarted;

    // ReSharper disable once ConvertToPrimaryConstructor
    public ProgressDataManager(IHubContext<ReCounterMessagesHub, IProgressDataMessenger> hub,
        ILogger<ProgressDataManager> logger)
    {
        _hub = hub;
        _logger = logger;
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        if (_timer != null)
        {
            await _timer.DisposeAsync();
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _timer?.Dispose();
    }

    public ProgressData? AccumulatedProgressData { get; private set; }

    public void UserConnected(string connectionId, string? userName)
    {
        Debug.WriteLine($"---===UserConnected connectionId={connectionId}, userName={userName}");
        if (userName is null)
        {
            return;
        }

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

    public void UserDisconnected(string connectionId, string? userName)
    {
        Debug.WriteLine($"---===UserDisconnected connectionId={connectionId}, userName={userName}");
        if (userName is null)
        {
            return;
        }

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

    public void StopTimer()
    {
        _timer?.Change(Timeout.Infinite, 0);
        _timerStarted = false;
        _logger.LogInformation("ProgressDataManager Timer stopped.");
    }

    public void Clear()
    {
        CheckTimer();
        lock (SyncRoot)
        {
            AccumulatedProgressData ??= new ProgressData();
            AccumulatedProgressData.Clear();
        }
        _currentChangeId++;
    }

    public async ValueTask SetProgressData(string? userName, string name, string message, bool instantly,
        CancellationToken cancellationToken = default)
    {
        CheckTimer();
        lock (SyncRoot)
        {
            AccumulatedProgressData ??= new ProgressData();
            AccumulatedProgressData.Add(name, message);
        }

        _currentChangeId++;

        if (instantly && _connectedUsers.Count > 0)
        {
            await SendData(userName, cancellationToken);
        }
    }

    public async ValueTask SetProgressData(string? userName, string name, bool value, bool instantly,
        CancellationToken cancellationToken = default)
    {
        CheckTimer();
        lock (SyncRoot)
        {
            AccumulatedProgressData ??= new ProgressData();
            AccumulatedProgressData.Add(name, value);
        }

        _currentChangeId++;

        if (instantly && _connectedUsers.Count > 0)
        {
            await SendData(userName, cancellationToken);
        }
    }

    public async ValueTask SetProgressData(string? userName, string name, int value, bool instantly,
        CancellationToken cancellationToken = default)
    {
        CheckTimer();
        lock (SyncRoot)
        {
            AccumulatedProgressData ??= new ProgressData();
            AccumulatedProgressData.Add(name, value);
        }

        _currentChangeId++;

        if (instantly && _connectedUsers.Count > 0)
        {
            await SendData(userName, cancellationToken);
        }
    }

    //implement destructor
    ~ProgressDataManager()
    {
        Dispose();
    }

    private void StartTimer()
    {
        _logger.LogInformation("ProgressDataManager Timer running.");
        _timerStarted = true;
        // ReSharper disable once DisposableConstructor
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
    }

    private void DoWork(object? state)
    {
        if (_sentChangeId == _currentChangeId)
        {
            return;
        }

        foreach (KeyValuePair<string, List<string>> user in _connectedUsers)
        {
            SendData(user.Key, CancellationToken.None).Wait();
        }
    }

    private async Task SendData(string? userName, CancellationToken cancellationToken = default)
    {
        //თუ მოთხოვნილია პროცესის შეჩერება, გამოვიდეთ მეთოდიდან
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }


        if (userName is null)
        {
            return;
        }

        if (!_connectedUsers.TryGetValue(userName, out List<string>? conList))
        {
            return;
        }

        _sentChangeId = _currentChangeId;

        //ყოველთვის იგზავნება სრული (აკუმულირებული) მდგომარეობა, რომ კლიენტმა ერთ შეტყობინებაში
        //მიიღოს ProcPosition-იც და ProcLength-იც. ასლი კეთდება lock-ის ქვეშ, რომ სერიალიზაციისას
        //პარალელურმა ცვლილებამ არ დააზიანოს.
        ProgressData? progressData;
        lock (SyncRoot)
        {
            progressData = AccumulatedProgressData is null
                ? null
                : new ProgressData
                {
                    BoolData = new Dictionary<string, bool>(AccumulatedProgressData.BoolData),
                    IntData = new Dictionary<string, int>(AccumulatedProgressData.IntData),
                    StrData = new Dictionary<string, string>(AccumulatedProgressData.StrData)
                };
        }

        if (progressData is not null)
        {
            foreach (string connectionId in conList)
            {
                //თუ მოთხოვნილია პროცესის შეჩერება, გამოვიდეთ მეთოდიდან
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                await _hub.Clients.Client(connectionId).SendProgressData(progressData, cancellationToken);
            }
        }
    }

    //private Task SendNotificationAsync(object objectToSend)
    //{
    //    return _hub.Clients.All.SendAsync("sendtoall", objectToSend);
    //}

    private void CheckTimer()
    {
        if (!_timerStarted && _connectedUsers.Count > 0)
        {
            StartTimer();
        }

        if (_timerStarted && _connectedUsers.Count == 0)
        {
            StopTimer();
        }
    }

    //private List<string>? CheckConnectionsAndTimer(string? userName)
    //{
    //    if (!_timerStarted && _connectedUsers.Count > 0)
    //        StartTimer();
    //    if (_timerStarted && _connectedUsers.Count == 0)
    //        StopTimer();
    //    return userName is null ? null : _connectedUsers.GetValueOrDefault(userName);
    //}
}
