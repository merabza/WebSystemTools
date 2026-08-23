using SystemTools.MediatRMessagingAbstractions;

namespace WebSystemTools.SignalRRecounterMessages.CommandRequests;

// ReSharper disable once ClassNeverInstantiated.Global
public record CancelCurrentProcessRequestCommand : ICommandOmd<bool>;
