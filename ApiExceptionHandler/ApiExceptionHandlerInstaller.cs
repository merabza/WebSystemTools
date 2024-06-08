using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Net.Mime;
using SystemToolsShared.Errors;
using WebInstallers;

namespace ApiExceptionHandler;

// ReSharper disable once UnusedType.Global
public sealed class ApiExceptionHandlerInstaller : IInstaller
{
    public int InstallPriority => 30;
    public int ServiceUsePriority => 30;

    public void InstallServices(WebApplicationBuilder builder, string[] args, Dictionary<string, string> parameters)
    {
    }

    public void UseServices(WebApplication app)
    {
        //Console.WriteLine("ApiExceptionHandlerInstaller.UseMiddleware Started");

        app.UseExceptionHandler(exceptionHandlerApp =>
        {
            exceptionHandlerApp.Run(async context =>
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                // using static System.Net.Mime.MediaTypeNames;
                context.Response.ContentType = MediaTypeNames.Text.Plain;

                //await context.Response.WriteAsync("An exception was thrown.");

                var exceptionHandlerPathFeature =
                    context.Features.Get<IExceptionHandlerPathFeature>();

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

        //Console.WriteLine("ApiExceptionHandlerInstaller.UseMiddleware Finished");
    }
}