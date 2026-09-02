using SystemTools.Application.Abstractions.Messaging;

namespace WebSystemTools.SignalRRecounterMessages.CommandRequests;

// ReSharper disable once ClassNeverInstantiated.Global
public record CancelCurrentProcessRequestCommand : ICommand<bool>;
