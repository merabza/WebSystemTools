using MessagingAbstractions;
using ReCounterContracts;

namespace SignalRRecounterMessages.QueryRequests;

// ReSharper disable once ClassNeverInstantiated.Global
public record CurrentProcessStatusQueryRequest : IQuery<ProgressData>;