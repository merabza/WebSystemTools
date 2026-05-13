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
public sealed class CurrentProcessStatusQueryHandler(IProgressDataManager progressDataManager)
    : IQueryHandler<CurrentProcessStatusRequestQuery, ProgressData>
{
    public Task<OneOf<ProgressData, Error[]>> Handle(CurrentProcessStatusRequestQuery request,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(
            OneOf<ProgressData, Error[]>.FromT0(progressDataManager.AccumulatedProgressData ?? new ProgressData()));
    }
}
