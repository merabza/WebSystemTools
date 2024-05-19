using MessagingAbstractions;

namespace SignalRMessages.QueryRequests;

// ReSharper disable once ClassNeverInstantiated.Global
public record IsProcessRunningQueryRequest : IQuery<bool>;