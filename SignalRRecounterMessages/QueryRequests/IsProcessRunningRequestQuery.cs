using SystemTools.MediatRMessagingAbstractions;

namespace SignalRRecounterMessages.QueryRequests;

// ReSharper disable once ClassNeverInstantiated.Global
public record IsProcessRunningRequestQuery : IQuery<bool>;