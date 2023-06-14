//using FluentValidation;
//using LanguageExt;
//using MediatR;
//using SystemToolsShared;

//namespace ApiToolsShared;

//public sealed class ValidationBehaviorReturnErrors<TRequest> : IPipelineBehavior<TRequest, Option<IEnumerable<Err>>>
//    where TRequest : ICommand<Option<IEnumerable<Err>>>, IQuery<Option<IEnumerable<Err>>>
//{

//    private readonly IEnumerable<IValidator<TRequest>> _validators;

//    public ValidationBehaviorReturnErrors(IEnumerable<IValidator<TRequest>> validators)
//    {
//        _validators = validators;
//    }

//    public async Task<Option<IEnumerable<Err>>> Handle(TRequest request, CancellationToken cancellationToken,
//        RequestHandlerDelegate<Option<IEnumerable<Err>>> next)
//    {
//        var context = new ValidationContext<TRequest>(request);
//        var failures = _validators.Select(x => x.Validate(context)).SelectMany(x => x.Errors).Where(x => x is not null)
//            .Select(x => new Err(x.ErrorCode, x.ErrorMessage)).ToArray();

//        if (failures.Any())
//            return await Task.FromResult(failures);

//        return await next();
//    }
//}

