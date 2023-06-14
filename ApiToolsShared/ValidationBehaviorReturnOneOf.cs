//using FluentValidation;
//using MediatR;
//using OneOf;
//using SystemToolsShared;

//namespace ApiToolsShared;

//public sealed class ValidationBehaviorReturnOneOf<TRequest, TResponse> : IPipelineBehavior<TRequest, OneOf<TResponse, IEnumerable<Err>>>
//    where TRequest : IQuery<OneOf<TResponse, IEnumerable<Err>>>, ICommand<OneOf<TResponse, IEnumerable<Err>>>
//{

//    private readonly IEnumerable<IValidator<TRequest>> _validators;

//    public ValidationBehaviorReturnOneOf(IEnumerable<IValidator<TRequest>> validators)
//    {
//        _validators = validators;
//    }

//    public async Task<OneOf<TResponse, IEnumerable<Err>>> Handle(TRequest request, CancellationToken cancellationToken,
//        RequestHandlerDelegate<OneOf<TResponse, IEnumerable<Err>>> next)
//    {
//        var context = new ValidationContext<TRequest>(request);
//        var failures = _validators.Select(x => x.Validate(context)).SelectMany(x => x.Errors).Where(x => x is not null)
//            .Select(x => new Err(x.ErrorCode, x.ErrorMessage)).ToArray();

//        if (failures.Any())
//            return await Task.FromResult(failures);

//        return await next();
//    }
//}

