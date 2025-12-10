using MediatRMessagingAbstractions;
using ReCounterContracts;

namespace SignalRRecounterMessages.QueryRequests;

// ReSharper disable once ClassNeverInstantiated.Global
public record CurrentProcessStatusRequestQuery : IQuery<ProgressData>;