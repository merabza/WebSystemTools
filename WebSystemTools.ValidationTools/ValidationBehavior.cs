//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Reflection;
//using System.Threading;
//using System.Threading.Tasks;
//using FluentValidation;
//using FluentValidation.Results;
//using MediatR;
//using OneOf;
//using SystemTools.SystemToolsShared.Errors;

//namespace WebSystemTools.ValidationTools;

//public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
//    where TRequest : IRequest<TResponse>
//{
//    // Cached per closed generic. Compiled once on first validation failure for this (TRequest, TResponse).
//    private static readonly Lazy<Func<ErrorOmd[], TResponse>> ErrorsToResponse = new(BuildErrorsToResponse);

//    private readonly IEnumerable<IValidator<TRequest>> _validators;

//    // ReSharper disable once ConvertToPrimaryConstructor
//    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
//    {
//        _validators = validators;
//    }

//    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
//        CancellationToken cancellationToken)
//    {
//        if (!_validators.Any())
//        {
//            return await next(cancellationToken);
//        }

//        var context = new ValidationContext<TRequest>(request);
//        List<ValidationFailure> failures =
//        [
//            .. _validators.Select(x => x.Validate(context)).SelectMany(x => x.Errors).Where(x => x is not null)
//        ];

//        if (failures.Count == 0)
//        {
//            return await next(cancellationToken);
//        }

//        ErrorOmd[] errors =
//            [.. failures.Select(x => new ErrorOmd { Code = x.ErrorCode, Name = x.ErrorMessage }).Distinct()];
//        return ErrorsToResponse.Value(errors);
//    }

//    // The behavior is registered as an open generic and is wired up for every request type.
//    // It only knows how to short-circuit when TResponse is OneOf<*, ErrorOmd[]>; for any other
//    // shape, surfacing a ValidationException is the safer default — silently letting an
//    // invalid request through would defeat the purpose of the pipeline.
//    private static Func<ErrorOmd[], TResponse> BuildErrorsToResponse()
//    {
//        Type t = typeof(TResponse);
//        if (!t.IsGenericType || t.GetGenericTypeDefinition() != typeof(OneOf<,>) ||
//            t.GetGenericArguments()[1] != typeof(ErrorOmd[]))
//        {
//            return _ => throw new ValidationException(
//                $"{nameof(ValidationBehavior<,>)} expects TResponse to be OneOf<*, ErrorOmd[]>; got '{t.FullName}'.");
//        }

//        MethodInfo fromT1 = t.GetMethod(nameof(OneOf<,>.FromT1), BindingFlags.Public | BindingFlags.Static) ??
//                            throw new InvalidOperationException($"OneOf<,>.FromT1 not found on '{t.FullName}'.");
//        ParameterExpression param = Expression.Parameter(typeof(ErrorOmd[]), "errors");
//        return Expression.Lambda<Func<ErrorOmd[], TResponse>>(Expression.Call(fromT1, param), param).Compile();
//    }
//}


