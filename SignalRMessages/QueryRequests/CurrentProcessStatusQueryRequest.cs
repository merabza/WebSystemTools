using MessagingAbstractions;
using ReCounterContracts;

namespace SignalRMessages.QueryRequests;

// ReSharper disable once ClassNeverInstantiated.Global
public record CurrentProcessStatusQueryRequest : IQuery<ProgressData>;