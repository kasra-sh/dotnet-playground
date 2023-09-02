#pragma warning disable CS1998

using Microsoft.Extensions.DependencyInjection;

namespace Infant.Core.Modularity;

public abstract class AppModule : IAppModule
{
    public virtual bool AutoRegisterDependencies { get; } = true;
    public virtual void ConfigureServices(IServiceCollection services)
    {
        ConfigureServicesAsync(services).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public virtual Task ConfigureServicesAsync(IServiceCollection services)
    {
        return Task.CompletedTask;
    }
}