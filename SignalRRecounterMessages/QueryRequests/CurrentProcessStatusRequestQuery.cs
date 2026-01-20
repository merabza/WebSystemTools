using SystemTools.MediatRMessagingAbstractions;
using SystemTools.ReCounterContracts;

namespace SignalRRecounterMessages.QueryRequests;

// ReSharper disable once ClassNeverInstantiated.Global
public record CurrentProcessStatusRequestQuery : IQuery<ProgressData>;