//using MediatR;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using OneOf;
//using SystemToolsShared;

//namespace ApiToolsShared;

//public sealed class ApiKeyCheckBehaviorReturnOneOf<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
//    where TRequest : ICommand<TResponse>
//{
//    private readonly IConfiguration _config;
//    private readonly HttpRequest _httpRequest;

//    private readonly ILogger _logger;

//    public ApiKeyCheckBehaviorReturnOneOf(ILogger<ApiKeyCheckBehaviorReturnOneOf<TRequest, TResponse>> logger,
//        HttpRequest httpRequest, IConfiguration config)
//    {
//        _logger = logger;
//        _httpRequest = httpRequest;
//        _config = config;
//    }

//    public async Task<OneOf<TResponse, IEnumerable<Err>>> Handle(TRequest request, CancellationToken cancellationToken,
//        RequestHandlerDelegate<OneOf<TResponse, IEnumerable<Err>>> next)
//    {
//        //if (request is null)
//        //    return await Task.FromResult(new[] { ApiErrors.RequestIsEmpty });
//        var apiKeysChecker = new ApiKeysChecker2(_logger, _config, _httpRequest);
//        var apiKeysCheckerResult = apiKeysChecker.Check();


//        return await apiKeysCheckerResult.MatchAsync(x => new[] { x }, () => next());
//        //return await apiKeysCheckerResult.MatchAsync(x => new[] { x }, () => next());
//        //return new[] { apiKeysCheckerResult. };
//    }
//}

