using SystemTools.Application.Abstractions.Messaging;

namespace WebSystemTools.SignalRRecounterMessages.QueryRequests;

// ReSharper disable once ClassNeverInstantiated.Global
public record IsProcessRunningRequestQuery : IQuery<bool>;
