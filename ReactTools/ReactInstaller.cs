////Created by ReactInstallerClassCreator at 8/1/2022 8:38:26 PM

//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using WebInstallers;

//namespace ReactTools;

//// ReSharper disable once UnusedType.Global
//public sealed class ReactInstaller : IInstaller
//{
//    public int InstallPriority => 15;
//    public int ServiceUsePriority => 135;

//    public void InstallServices(WebApplicationBuilder builder, string[] args)
//    {
//        Console.WriteLine("ReactInstaller.InstallServices Started");

//        // In production, the React files will be served from this directory
//        builder.Services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/build"; });

//        Console.WriteLine("ReactInstaller.InstallServices Finished");
//    }

//    public void UseServices(WebApplication app)
//    {
//        Console.WriteLine("ReactInstaller.UseMiddleware Started");

//        if (app.Environment.IsDevelopment())
//            app.UseDeveloperExceptionPage();
//        else
//            app.UseExceptionHandler("/Error");

//        app.UseStaticFiles();
//        app.UseSpaStaticFiles();

//        app.UseSpa(spa =>
//        {
//            spa.Options.SourcePath = "ClientApp";
//            if (app.Environment.IsDevelopment()) spa.UseReactDevelopmentServer("start");
//        });

//        Console.WriteLine("ReactInstaller.UseMiddleware Finished");
//    }
//}

