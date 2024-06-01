////Created by ProjectBackgroundTasksQueueInstallerClassCreator at 8/3/2022 10:20:24 PM

//using System.Collections.Generic;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.Extensions.DependencyInjection;
//using WebInstallers;

//namespace BackgroundTasksTools;

//// ReSharper disable once UnusedType.Global
//public sealed class BackgroundTasksQueueInstaller : IInstaller
//{
//    public int InstallPriority => 30;
//    public int ServiceUsePriority => 30;

//    public void InstallServices(WebApplicationBuilder builder, string[] args, Dictionary<string, string> parameters)
//    {
//        //Console.WriteLine("BackgroundTasksQueueInstaller.InstallServices Started");

//        //builder.Services.AddHostedService<ModelPartQueuedHostedService>()
//        //builder.Services.AddSingleton<IModelPartBackgroundTaskQueue, ModelPartBackgroundTaskQueue>()
//        builder.Services.AddSignalR();

//        //Console.WriteLine("BackgroundTasksQueueInstaller.InstallServices Finished");
//    }

//    public void UseServices(WebApplication app)
//    {
//    }
//}

