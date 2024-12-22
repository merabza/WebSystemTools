using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using ReCounterContracts;
using ReCounterDom;

namespace SignalRRecounterMessages;

public class ProgressDataManager : IProgressDataManager, IDisposable, IAsyncDisposable
{
    private static readonly Lock SyncRoot = new();


    private static int _sentCount;
    private readonly Dictionary<string, List<string>> _connectedUsers = [];
    private readonly IHubContext<ReCounterMessagesHub, IProgressDataMessenger> _hub;
    private readonly ILogger<ProgressDataManager> _logger;
    private int _currentChangeId;
    private ProgressData? _lastChangesData;
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
        if (_timer != null) await _timer.DisposeAsync();
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
            return;
        if (!_connectedUsers.ContainsKey(userName))
            _connectedUsers.Add(userName, []);
        var conList = _connectedUsers[userName];
        if (!conList.Contains(connectionId))
            conList.Add(connectionId);
    }

    public void UserDisconnected(string connectionId, string? userName)
    {
        Debug.WriteLine($"---===UserDisconnected connectionId={connectionId}, userName={userName}");
        if (userName is null)
            return;
        if (!_connectedUsers.TryGetValue(userName, out var conList))
            return;
        if (!conList.Contains(connectionId))
            return;
        conList.Remove(connectionId);
        if (conList.Count == 0)
            _connectedUsers.Remove(userName);
    }

    public void StopTimer()
    {
        _timer?.Change(Timeout.Infinite, 0);
        _timerStarted = false;
        _logger.LogInformation("ProgressDataManager Timer stopped.");
    }


    public async Task SetProgressData(string? userName, string name, string message, bool instantly,
        CancellationToken cancellationToken = default)
    {
        CheckTimer();
        lock (SyncRoot)
        {
            AccumulatedProgressData ??= new ProgressData();
            AccumulatedProgressData.Add(name, message);
            _lastChangesData = new ProgressData();
            _lastChangesData.Add(name, message);
        }

        _currentChangeId++;

        if (instantly && _connectedUsers.Count > 0)
            await SendData(userName, cancellationToken);
    }

    public async Task SetProgressData(string? userName, string name, bool value, bool instantly,
        CancellationToken cancellationToken = default)
    {
        CheckTimer();
        lock (SyncRoot)
        {
            AccumulatedProgressData ??= new ProgressData();
            AccumulatedProgressData.Add(name, value);
            _lastChangesData = new ProgressData();
            _lastChangesData.Add(name, value);
        }

        _currentChangeId++;

        if (instantly && _connectedUsers.Count > 0)
            await SendData(userName, cancellationToken);
    }

    public async Task SetProgressData(string? userName, string name, int value, bool instantly,
        CancellationToken cancellationToken = default)
    {
        CheckTimer();
        lock (SyncRoot)
        {
            AccumulatedProgressData ??= new ProgressData();
            AccumulatedProgressData.Add(name, value);
            _lastChangesData = new ProgressData();
            _lastChangesData.Add(name, value);
        }

        _currentChangeId++;

        if (instantly && _connectedUsers.Count > 0)
            await SendData(userName, cancellationToken);
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
            return;
        foreach (var user in _connectedUsers)
            SendData(user.Key, CancellationToken.None).Wait();
    }

    private async Task SendData(string? userName, CancellationToken cancellationToken = default)
    {
        if (userName is null)
            return;

        if (!_connectedUsers.TryGetValue(userName, out var conList))
            return;

        _sentChangeId = _currentChangeId;
        _sentCount++;
        var progressData = _lastChangesData;
        if (_sentCount % 10 == 0)
            progressData = AccumulatedProgressData;
        if (progressData is not null)
            foreach (var connectionId in conList)
                await _hub.Clients.Client(connectionId).SendProgressData(progressData, cancellationToken);
    }

    //private Task SendNotificationAsync(object objectToSend)
    //{
    //    return _hub.Clients.All.SendAsync("sendtoall", objectToSend);
    //}


    private void CheckTimer()
    {
        if (!_timerStarted && _connectedUsers.Count > 0)
            StartTimer();
        if (_timerStarted && _connectedUsers.Count == 0)
            StopTimer();
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