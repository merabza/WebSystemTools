using System.Threading;
using System.Threading.Tasks;
using MediatRMessagingAbstractions;
using OneOf;
using ReCounterAbstraction;
using ReCounterContracts;
using SignalRRecounterMessages.QueryRequests;
using SystemToolsShared.Errors;

namespace SignalRRecounterMessages.Handlers;

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
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            OneOf<ProgressData, Err[]>.FromT0(_progressDataManager.AccumulatedProgressData ?? new ProgressData()));
    }
}