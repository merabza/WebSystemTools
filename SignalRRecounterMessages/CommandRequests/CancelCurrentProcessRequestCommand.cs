using MediatRMessagingAbstractions;

namespace SignalRRecounterMessages.CommandRequests;

// ReSharper disable once ClassNeverInstantiated.Global
public record CancelCurrentProcessRequestCommand : ICommand<bool>;