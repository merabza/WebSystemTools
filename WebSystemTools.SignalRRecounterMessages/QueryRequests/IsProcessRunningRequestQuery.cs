using SystemTools.MediatRMessagingAbstractions;

namespace WebSystemTools.SignalRRecounterMessages.QueryRequests;

// ReSharper disable once ClassNeverInstantiated.Global
public record IsProcessRunningRequestQuery : IQuery<bool>;
