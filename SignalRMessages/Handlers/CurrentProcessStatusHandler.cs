using MessagingAbstractions;
using OneOf;
using ReCounterContracts;
using ReCounterDom;
using SignalRMessages.QueryRequests;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SystemToolsShared.Errors;

namespace SignalRMessages.Handlers;

// ReSharper disable once ClassNeverInstantiated.Global
public class CurrentProcessStatusHandler : IQueryHandler<CurrentProcessStatusQueryRequest, ProgressData>
{
    private readonly IProgressDataManager _progressDataManager;

    // ReSharper disable once ConvertToPrimaryConstructor
    public CurrentProcessStatusHandler(IProgressDataManager progressDataManager)
    {
        _progressDataManager = progressDataManager;
    }

    public Task<OneOf<ProgressData, IEnumerable<Err>>> Handle(CurrentProcessStatusQueryRequest request,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(
            OneOf<ProgressData, IEnumerable<Err>>.FromT0(_progressDataManager.AccumulatedProgressData ??
                                                         new ProgressData()));
    }
}