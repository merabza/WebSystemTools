using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using ReCounterContracts;
using ReCounterDom;

namespace SignalRMessages;

public class ProgressDataManager : IProgressDataManager, IDisposable, IAsyncDisposable
{
    private static readonly object SyncRoot = new();
    private readonly HashSet<string> _connectedIds = [];
    private readonly IHubContext<ReCounterMessagesHub, IProgressDataMessenger> _hub;
    private readonly ILogger<ProgressDataManager> _logger;
    private int _currentChangeId;
    private ProgressData? _accumulatedProgressData;
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

    public void UserConnected(string connectionId)
    {
        _connectedIds.Add(connectionId);
    }

    public void UserDisconnected(string connectionId)
    {
        _connectedIds.Remove(connectionId);
    }

    public void StopTimer()
    {
        _timer?.Change(Timeout.Infinite, 0);
        _timerStarted = false;
        _logger.LogInformation("ProgressDataManager Timer stopped.");
    }


    public async Task SetProgressData(string name, string message, bool instantly, CancellationToken cancellationToken)
    {
        CheckTimer();
        lock (SyncRoot)
        {
            _accumulatedProgressData ??= new ProgressData();
            _accumulatedProgressData.Add(name, message);
            _lastChangesData ??= new ProgressData();
            _lastChangesData.Add(name, message);
            _currentChangeId++;
        }

        if (instantly && _connectedIds.Count > 0)
            await SendData(cancellationToken);
    }

    public async Task SetProgressData(string name, bool value, bool instantly, CancellationToken cancellationToken)
    {
        lock (SyncRoot)
        {
            _accumulatedProgressData ??= new ProgressData();
            _accumulatedProgressData.Add(name, value);
            _lastChangesData ??= new ProgressData();
            _lastChangesData.Add(name, value);
            _currentChangeId++;
        }

        if (instantly && _connectedIds.Count > 0)
            await SendData(cancellationToken);
    }

    public async Task SetProgressData(string name, int value, bool instantly, CancellationToken cancellationToken)
    {
        CheckTimer();
        lock (SyncRoot)
        {
            _accumulatedProgressData ??= new ProgressData();
            _accumulatedProgressData.Add(name, value);
            _lastChangesData ??= new ProgressData();
            _lastChangesData.Add(name, value);
            _currentChangeId++;
        }

        if (instantly && _connectedIds.Count > 0)
            await SendData(cancellationToken);
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
        SendData(CancellationToken.None).Wait();
    }


    private static int _sentCount;

    private async Task SendData(CancellationToken cancellationToken)
    {
        _sentChangeId = _currentChangeId;
        _sentCount++;
        var progressData = _lastChangesData;
        if (_sentCount % 10 == 0)
            progressData = _accumulatedProgressData;
        if (progressData is not null)
            await _hub.Clients.All.SendProgressData(progressData, cancellationToken);
    }

    //private Task SendNotificationAsync(object objectToSend)
    //{
    //    return _hub.Clients.All.SendAsync("sendtoall", objectToSend);
    //}

    private void CheckTimer()
    {
        if (!_timerStarted && _connectedIds.Count > 0)
            StartTimer();
        if (_timerStarted && _connectedIds.Count == 0)
            StopTimer();
    }
}