using System.Threading;
using System.Threading.Tasks;
using OneOf;
using SystemTools.MediatRMessagingAbstractions;
using SystemTools.ReCounterAbstraction;
using SystemTools.ReCounterContracts;
using SystemTools.SystemToolsShared.Errors;
using WebSystemTools.SignalRRecounterMessages.QueryRequests;

namespace WebSystemTools.SignalRRecounterMessages.Handlers;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class CurrentProcessStatusQueryHandler : IQueryHandler<CurrentProcessStatusRequestQuery, ProgressData>
{
    private readonly IProgressDataManager _progressDataManager;

    // ReSharper disable once ConvertToPrimaryConstructor
    public CurrentProcessStatusQueryHandler(IProgressDataManager progressDataManager)
    {
        _progressDataManager = progressDataManager;
    }

    public Task<OneOf<ProgressData, Err[]>> Handle(CurrentProcessStatusRequestQuery request,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(
            OneOf<ProgressData, Err[]>.FromT0(_progressDataManager.AccumulatedProgressData ?? new ProgressData()));
    }
}
