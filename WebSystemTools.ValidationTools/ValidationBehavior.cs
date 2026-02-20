using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using OneOf;
using SystemTools.MediatRMessagingAbstractions;
using SystemTools.SystemToolsShared.Errors;
using ICommand = System.Windows.Input.ICommand;

namespace WebSystemTools.ValidationTools;

public sealed class
    ValidationBehavior<TCommandOrQuery, TResponse> : IPipelineBehavior<TCommandOrQuery, OneOf<TResponse, Err[]>>
    where TCommandOrQuery : ICommand, ICommand<TResponse>, IQuery<TResponse>
{
    private readonly IEnumerable<IValidator<TCommandOrQuery>> _validators;

    // ReSharper disable once ConvertToPrimaryConstructor
    public ValidationBehavior(IEnumerable<IValidator<TCommandOrQuery>> validators)
    {
        _validators = validators;
    }

    public async Task<OneOf<TResponse, Err[]>> Handle(TCommandOrQuery request,
        RequestHandlerDelegate<OneOf<TResponse, Err[]>> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next(cancellationToken);
        }

        var context = new ValidationContext<TCommandOrQuery>(request);
        List<ValidationFailure> failures = _validators.Select(x => x.Validate(context)).SelectMany(x => x.Errors)
            .Where(x => x is not null).ToList();

        if (failures.Count != 0)
        {
            return await Task.FromResult(failures
                .Select(x => new Err { ErrorCode = x.ErrorCode, ErrorMessage = x.ErrorMessage }).Distinct().ToArray());
        }

        return await next(cancellationToken);
    }
}
