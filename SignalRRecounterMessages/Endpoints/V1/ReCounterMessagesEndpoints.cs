using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using ReCounterContracts.V1.Routes;
using ReCounterDom;
using SignalRRecounterMessages.CommandRequests;
using SignalRRecounterMessages.Handlers;
using SignalRRecounterMessages.QueryRequests;
using WebInstallers;

namespace SignalRRecounterMessages.Endpoints.V1;

// ReSharper disable once UnusedType.Global
public sealed class ReCounterMessagesEndpoints : IInstaller
{
    public int InstallPriority => 70;

    public int ServiceUsePriority => 70;

    public bool InstallServices(WebApplicationBuilder builder, bool debugMode, string[] args,
        Dictionary<string, string> parameters)
    {
        return true;
    }

    public bool UseServices(WebApplication app, bool debugMode)
    {
        if (debugMode)
            Console.WriteLine($"{GetType().Name}.{nameof(UseServices)} Started");

        var group = app
            .MapGroup(RecountMessagesRoutes.ApiBase +
                      RecountMessagesRoutes.ReCounterRoute.MessagesRoute).RequireAuthorization();

        group.MapHub<ReCounterMessagesHub>(RecountMessagesRoutes.ReCounterRoute.RecountMessages,
            options => { options.Transports = /*HttpTransportType.WebSockets | */HttpTransportType.LongPolling; });

        // POST api/v1/recounter/cancelcurrentprocess
        group.MapPost(RecountMessagesRoutes.ReCounterRoute.CancelCurrentProcess, CancelCurrentProcess);
        // GET api/v1/recounter/currentprocessstatus
        group.MapGet(RecountMessagesRoutes.ReCounterRoute.CurrentProcessStatus, CurrentProcessStatus);
        // POST api/v1/recounter/isprocessrunning
        group.MapPost(RecountMessagesRoutes.ReCounterRoute.IsProcessRunning, IsProcessRunning);
        // POST api/v1/recounter/clearredundantlemmas

        if (debugMode)
            Console.WriteLine($"{GetType().Name}.{nameof(UseServices)} Finished");

        return true;
    }

    // GET api/v1/recounter/currentprocessstatus
    private static async Task<IResult> CurrentProcessStatus(HttpRequest httpRequest, IMediator mediator,
        IProgressDataManager messagesDataManager, CancellationToken cancellationToken)
    {
        var userName = httpRequest.HttpContext.User.Identity?.Name;
        //await messagesDataManager.SendMessage(userName, $"{nameof(CurrentProcessStatus)} started", true, cancellationToken);
        Debug.WriteLine($"Call {nameof(CurrentProcessStatusHandler)} from {nameof(CurrentProcessStatus)}");

        var query = new CurrentProcessStatusQueryRequest();
        var result = await mediator.Send(query, cancellationToken);

        //await messagesDataManager.SendMessage(userName, $"{nameof(CurrentProcessStatus)} finished", cancellationToken);
        return result.Match(Results.Ok, Results.BadRequest);
    }

    // POST api/v1/recounter/isprocessrunning
    private static async Task<IResult> IsProcessRunning(HttpRequest httpRequest, IMediator mediator,
        IProgressDataManager messagesDataManager, CancellationToken cancellationToken)
    {
        var userName = httpRequest.HttpContext.User.Identity?.Name;
        //await messagesDataManager.SendMessage(userName, $"{nameof(IsProcessRunning)} started", cancellationToken);
        Debug.WriteLine($"Call {nameof(IsProcessRunningHandler)} from {nameof(IsProcessRunning)}");

        var query = new IsProcessRunningQueryRequest();
        var result = await mediator.Send(query, cancellationToken);

        //await messagesDataManager.SendMessage(userName, $"{nameof(IsProcessRunning)} finished", cancellationToken);
        return result.Match(Results.Ok, Results.BadRequest);
    }

    // POST api/v1/recounter/cancelcurrentprocess
    private static async Task<IResult> CancelCurrentProcess(HttpRequest httpRequest, IMediator mediator,
        IProgressDataManager messagesDataManager, CancellationToken cancellationToken)
    {
        var userName = httpRequest.HttpContext.User.Identity?.Name;
        //await messagesDataManager.SendMessage(userName, $"{nameof(CancelCurrentProcess)} started", cancellationToken);
        Debug.WriteLine($"Call {nameof(CancelCurrentProcessHandler)} from {nameof(CancelCurrentProcess)}");

        var query = new CancelCurrentProcessCommandRequest();
        var result = await mediator.Send(query, cancellationToken);

        //await messagesDataManager.SendMessage(userName, $"{nameof(CancelCurrentProcess)} finished", cancellationToken);
        return result.Match(Results.Ok, Results.BadRequest);
    }
}