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
    : IQueryHandlerOmd<CurrentProcessStatusRequestQuery, ProgressData>
{
    public Task<OneOf<ProgressData, ErrorOmd[]>> Handle(CurrentProcessStatusRequestQuery request,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(
            OneOf<ProgressData, ErrorOmd[]>.FromT0(progressDataManager.AccumulatedProgressData ?? new ProgressData()));
    }
}
