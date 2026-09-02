using System.Threading;
using System.Threading.Tasks;
using SystemTools.Application.Abstractions.Messaging;
using SystemTools.ReCounterAbstraction;
using SystemTools.ReCounterContracts;
using SystemTools.SharedKernel;
using WebSystemTools.SignalRRecounterMessages.QueryRequests;

namespace WebSystemTools.SignalRRecounterMessages.Handlers;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class CurrentProcessStatusQueryHandler(IProgressDataManager progressDataManager)
    : IQueryHandler<CurrentProcessStatusRequestQuery, ProgressData>
{
    public Task<Result<ProgressData>> Handle(CurrentProcessStatusRequestQuery request,
        CancellationToken cancellationToken)
    {
        return Task.FromResult<Result<ProgressData>>(progressDataManager.AccumulatedProgressData ?? new ProgressData());
    }
}
