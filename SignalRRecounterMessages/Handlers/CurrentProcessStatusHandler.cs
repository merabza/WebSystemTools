using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatRMessagingAbstractions;
using OneOf;
using ReCounterContracts;
using ReCounterDom;
using SignalRRecounterMessages.QueryRequests;
using SystemToolsShared.Errors;

namespace SignalRRecounterMessages.Handlers;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class CurrentProcessStatusHandler : IQueryHandler<CurrentProcessStatusQueryRequest, ProgressData>
{
    private readonly IProgressDataManager _progressDataManager;

    // ReSharper disable once ConvertToPrimaryConstructor
    public CurrentProcessStatusHandler(IProgressDataManager progressDataManager)
    {
        _progressDataManager = progressDataManager;
    }

    public Task<OneOf<ProgressData, Err[]>> Handle(CurrentProcessStatusQueryRequest request,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            OneOf<ProgressData, Err[]>.FromT0(_progressDataManager.AccumulatedProgressData ??
                                                         new ProgressData()));
    }
}