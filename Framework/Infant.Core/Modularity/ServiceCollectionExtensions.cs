using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infant.Core.Modularity;

public static class ServiceCollectionExtensions
{
    public static T GetSingletonInstanceOrNull<T>(this IServiceCollection services)
    {
        return (T)services
            .FirstOrDefault(d => d.ServiceType == typeof(T))
            ?.ImplementationInstance;
    }

    public static T GetSingletonInstance<T>(this IServiceCollection services)
    {
        var service = services.GetSingletonInstanceOrNull<T>();
        if (service == null)
        {
            throw new InvalidOperationException("Could not find singleton service: " + typeof(T).AssemblyQualifiedName);
        }

        return service;
    }

    public static IConfiguration GetConfiguration(this IServiceCollection services)
    {
        return GetSingletonInstance<IConfiguration>(services);
    }

    public static void AddApplicationModule<TModule>(this IServiceCollection serviceCollection)
    {
        var applicationManager = new ApplicationManager(serviceCollection, typeof(TModule));
        serviceCollection.AddSingleton(applicationManager);
        applicationManager.RegisterApplicationModules();
        // return serviceCollection;
    }
}