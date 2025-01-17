using FluentValidation;
using MediatR;
using MessagingAbstractions;
using OneOf;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SystemToolsShared.Errors;

namespace FluentValidationInstaller;

public sealed class
    ValidationBehavior<TCommandOrQuery, TResponse> : IPipelineBehavior<TCommandOrQuery,
    OneOf<TResponse, IEnumerable<Err>>>
    where TCommandOrQuery : ICommand, ICommand<TResponse>, IQuery<TResponse>
{
    private readonly IEnumerable<IValidator<TCommandOrQuery>> _validators;

    // ReSharper disable once ConvertToPrimaryConstructor
    public ValidationBehavior(IEnumerable<IValidator<TCommandOrQuery>> validators)
    {
        _validators = validators;
    }

    public async Task<OneOf<TResponse, IEnumerable<Err>>> Handle(TCommandOrQuery request,
        RequestHandlerDelegate<OneOf<TResponse, IEnumerable<Err>>> next, CancellationToken cancellationToken = default)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TCommandOrQuery>(request);
        var failures = _validators.Select(x => x.Validate(context)).SelectMany(x => x.Errors).Where(x => x is not null)
            .ToList();

        if (failures.Count != 0)
            return await Task.FromResult(failures
                .Select(x => new Err { ErrorCode = x.ErrorCode, ErrorMessage = x.ErrorMessage }).Distinct().ToArray());

        return await next();
    }
}