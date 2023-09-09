using Autofac;
using Autofac.Extensions.DependencyInjection;
using Infant.Core.Modularity;
using AutofacServiceProviderFactory = Infant.Core.Modularity.AutofacServiceProviderFactory;

// using Microsoft.AspNetCore.Builder;

namespace Infant.Host;

public class WebApplicationSettings
{
    public bool AddSwaggerUiForDevelopment { get; set; } = false;
    public bool AddSwaggerUiForProduction { get; set; } = false;
    public bool AddControllers { get; set; } = false;
}

public static class WebApplicationExtensions
{
    public static async Task AddApplicationAsync<TModule>(
        this WebApplicationBuilder webAppBuilder)
    {
        var containerBuilder = new ContainerBuilder();
        webAppBuilder.Services.AddSingleton(containerBuilder);
        webAppBuilder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory(containerBuilder));

        webAppBuilder.Services.AddApplicationModule<TModule>(webAppBuilder);
    }
    
    public static async Task InitializeApplicationAsync(this WebApplication app)
    {
        var appModules = app.Services.GetServices<AppModule>();

        foreach (var appModule in appModules)
        {
            if (CallAndCheckIsNotImplemented(() => appModule.OnApplicationInitialization(app.Services)))
            {
                await CallAndCheckIsNotImplementedAsync(async () => await appModule.OnApplicationInitializationAsync(app.Services));
            }

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