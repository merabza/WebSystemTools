using System.Net.Mime;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using SystemTools.SystemToolsShared.Errors;

namespace WebSystemTools.ApiExceptionHandler.DependencyInjection;

// ReSharper disable once UnusedType.Global
public static class ApiExceptionHandlerDependencyInjection
{
    public static bool UseApiExceptionHandler(this IApplicationBuilder app, ILogger? debugLogger)
    {
        debugLogger?.Information("{MethodName} Started", nameof(UseApiExceptionHandler));

        app.UseExceptionHandler(exceptionHandlerApp =>
        {
            exceptionHandlerApp.Run(async context =>
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                // using static System.Net.Mime.MediaTypeNames;
                context.Response.ContentType = MediaTypeNames.Text.Plain;

                //await context.Response.WriteAsync("An exception was thrown.");

                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

                var e = exceptionHandlerPathFeature?.Error;
                if (e is not null)
                {
                    var mess = new[] { SystemToolsErrors.UnexpectedApiException(e) };
                    var serializerSettings = new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    };
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(mess, serializerSettings));
                }
                //return Results.BadRequest(e.Message + Environment.NewLine + e.StackTrace);
            });
        });

        debugLogger?.Information("{MethodName} Finished", nameof(UseApiExceptionHandler));

        return true;
    }
}
