using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Routing;
using ReCounterAbstraction;
using SignalRRecounterMessages.CommandRequests;
using SignalRRecounterMessages.Handlers;
using SignalRRecounterMessages.QueryRequests;
using SystemTools.ReCounterContracts.V1.Routes;

namespace SignalRRecounterMessages.Endpoints.V1;

// ReSharper disable once UnusedType.Global
public static class ReCounterMessagesEndpoints
{
    public static bool MapReCounterMessagesEndpoints(this IEndpointRouteBuilder endpoints, bool debugMode)
    {
        if (debugMode)
            Console.WriteLine($"{nameof(MapReCounterMessagesEndpoints)} Started");

        var group = endpoints.MapGroup(RecountMessagesRoutes.ApiBase + RecountMessagesRoutes.ReCounterRoute.Recounter)
            .RequireAuthorization();

        group.MapHub<ReCounterMessagesHub>(RecountMessagesRoutes.ReCounterRoute.Messages,
            options => { options.Transports = /*HttpTransportType.WebSockets | */HttpTransportType.LongPolling; });

        // POST api/v1/recounter/cancelcurrentprocess
        group.MapPost(RecountMessagesRoutes.ReCounterRoute.CancelCurrentProcess, CancelCurrentProcess);
        // GET api/v1/recounter/currentprocessstatus
        group.MapGet(RecountMessagesRoutes.ReCounterRoute.CurrentProcessStatus, CurrentProcessStatus);
        // POST api/v1/recounter/isprocessrunning
        group.MapPost(RecountMessagesRoutes.ReCounterRoute.IsProcessRunning, IsProcessRunning);
        // POST api/v1/recounter/clearredundantlemmas

        if (debugMode)
            Console.WriteLine($"{nameof(MapReCounterMessagesEndpoints)} Finished");

        return true;
    }

    // GET api/v1/recounter/currentprocessstatus
    private static async Task<IResult> CurrentProcessStatus(IMediator mediator,
        IProgressDataManager messagesDataManager, CancellationToken cancellationToken = default)
    {
        //var userName = httpRequest.HttpContext.User.Identity?.Name;
        //await messagesDataManager.SendMessage(userName, $"{nameof(CurrentProcessStatus)} started", true, cancellationToken);
        Debug.WriteLine($"Call {nameof(CurrentProcessStatusQueryHandler)} from {nameof(CurrentProcessStatus)}");

        var query = new CurrentProcessStatusRequestQuery();
        var result = await mediator.Send(query, cancellationToken);

        //await messagesDataManager.SendMessage(userName, $"{nameof(CurrentProcessStatus)} finished", cancellationToken);
        return result.Match(Results.Ok, Results.BadRequest);
    }

    // POST api/v1/recounter/isprocessrunning
    private static async Task<IResult> IsProcessRunning(IMediator mediator, IProgressDataManager messagesDataManager,
        CancellationToken cancellationToken = default)
    {
        //var userName = httpRequest.HttpContext.User.Identity?.Name;
        //await messagesDataManager.SendMessage(userName, $"{nameof(IsProcessRunning)} started", cancellationToken);
        Debug.WriteLine($"Call {nameof(IsProcessRunningQueryHandler)} from {nameof(IsProcessRunning)}");

        var query = new IsProcessRunningRequestQuery();
        var result = await mediator.Send(query, cancellationToken);

        //await messagesDataManager.SendMessage(userName, $"{nameof(IsProcessRunning)} finished", cancellationToken);
        return result.Match(Results.Ok, Results.BadRequest);
    }

    // POST api/v1/recounter/cancelcurrentprocess
    private static async Task<IResult> CancelCurrentProcess(IMediator mediator,
        IProgressDataManager messagesDataManager, CancellationToken cancellationToken = default)
    {
        //var userName = httpRequest.HttpContext.User.Identity?.Name;
        //await messagesDataManager.SendMessage(userName, $"{nameof(CancelCurrentProcess)} started", cancellationToken);
        Debug.WriteLine($"Call {nameof(CancelCurrentProcessCommandHandler)} from {nameof(CancelCurrentProcess)}");

        var query = new CancelCurrentProcessRequestCommand();
        var result = await mediator.Send(query, cancellationToken);

        //await messagesDataManager.SendMessage(userName, $"{nameof(CancelCurrentProcess)} finished", cancellationToken);
        return result.Match(Results.Ok, Results.BadRequest);
    }
}