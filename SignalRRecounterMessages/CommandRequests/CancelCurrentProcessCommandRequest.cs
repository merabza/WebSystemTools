using MessagingAbstractions;

namespace SignalRRecounterMessages.CommandRequests;

// ReSharper disable once ClassNeverInstantiated.Global
public record CancelCurrentProcessCommandRequest : ICommand<bool>;