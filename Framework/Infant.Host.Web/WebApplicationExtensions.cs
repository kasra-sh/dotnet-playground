using Autofac;
using Infant.Core.Modularity;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using AutofacServiceProviderFactory = Infant.Core.Modularity.AutofacServiceProviderFactory;

// using Microsoft.AspNetCore.Builder;

namespace Infant.Host;

public static class WebApplicationExtensions
{
    public static async Task AddApplicationAsync<TModule>(
        this WebApplicationBuilder webAppBuilder, WebApplicationSettings settings = null)
    {
        if (Log.Logger.GetType().Name == "SilentLogger")
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(theme: AnsiConsoleTheme.Sixteen)
                .CreateLogger();
        }

        if (settings is null)
        {
            settings = new WebApplicationSettings();
        }

        webAppBuilder.Services.AddSingleton(settings);
        
        var containerBuilder = new ContainerBuilder();
        webAppBuilder.Services.AddSingleton(containerBuilder);
        webAppBuilder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory(containerBuilder));
        webAppBuilder.Services.AddApplicationModule<TModule>(webAppBuilder);
    }
    
    public static async Task InitializeApplicationAsync(this WebApplication app)
    {
        var appModules = app.Services.GetServices<AppModule>().ToArray();

        foreach (var appModule in appModules)
        {
            if (CallAndCheckIsNotImplemented(() => appModule.OnApplicationInitialization(app.Services)))
            {
                await CallAndCheckIsNotImplementedAsync(async () => await appModule.OnApplicationInitializationAsync(app.Services));
            }
        }
        
        foreach (var appModule in appModules.Reverse())
        {
            
            app.Lifetime.ApplicationStopping.Register(() => { CallAndCheckIsNotImplemented(() => appModule.OnApplicationShutdown(app.Services)); });
        }
    }
    
    private static bool CallAndCheckIsNotImplemented(Action action)
    {
        try
        {
            action.Invoke();
            return false;
        }
        catch (NotImplementedException)
        {
            return true;
        }
    }
    
    private static async Task<bool> CallAndCheckIsNotImplementedAsync(Func<Task> action)
    {
        try
        {
            await action.Invoke();
            return false;
        }
        catch (NotImplementedException)
        {
            return true;
        }
    }

}