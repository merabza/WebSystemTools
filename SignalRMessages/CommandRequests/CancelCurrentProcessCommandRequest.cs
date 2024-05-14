using MessagingAbstractions;

namespace SignalRMessages.CommandRequests;

// ReSharper disable once ClassNeverInstantiated.Global
public record CancelCurrentProcessCommandRequest : ICommand<bool>;