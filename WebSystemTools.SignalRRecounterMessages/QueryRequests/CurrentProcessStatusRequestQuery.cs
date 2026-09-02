using SystemTools.Application.Abstractions.Messaging;
using SystemTools.ReCounterContracts;

namespace WebSystemTools.SignalRRecounterMessages.QueryRequests;

// ReSharper disable once ClassNeverInstantiated.Global
public record CurrentProcessStatusRequestQuery : IQuery<ProgressData>;
