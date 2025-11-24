#pragma warning disable CS1998

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Boiler.Core.Modularity;

public abstract class AppModule
{
    public virtual bool AutoRegisterDependencies => true;

    public virtual void ConfigureServices(IServiceCollection services)
    {
        ConfigureServicesAsync(services).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public virtual Task ConfigureServicesAsync(IServiceCollection services)
    {
        return Task.CompletedTask;
    }
    
    public virtual void OnApplicationInitialization(IServiceProvider services)
    {
        throw new NotImplementedException();
    }

    public virtual async Task OnApplicationInitializationAsync(IServiceProvider services)
    {
        throw new NotImplementedException();
    }

    public virtual void OnApplicationShutdown(IServiceProvider services)
    {
        throw new NotImplementedException();
    }
}