using MediatRMessagingAbstractions;

namespace SignalRRecounterMessages.QueryRequests;

// ReSharper disable once ClassNeverInstantiated.Global
public record IsProcessRunningQueryRequest : IQuery<bool>;