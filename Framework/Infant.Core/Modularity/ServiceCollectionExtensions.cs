using Autofac;
using Infant.Core.Ioc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infant.Core.Modularity;

public static class ServiceCollectionExtensions
{
    public static T GetSingletonInstanceOrNull<T>(this IServiceCollection services)
    {
        return (T)services
            .FirstOrDefault(d => d.ServiceType == typeof(T) && d.ImplementationInstance != null)
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

    public static void AddApplicationModule<TModule>(this IServiceCollection serviceCollection, WebApplicationBuilder webApplicationBuilder)
    {
        var applicationManager = new AppModuleManager(serviceCollection, typeof(TModule));
        serviceCollection.AddSingleton(applicationManager);
        applicationManager.RegisterApplicationModules(webApplicationBuilder.Configuration);
    }
    
    public static ObjectAccessor<T> AddObjectAccessor<T>(this IServiceCollection services)
    {
        return services.AddObjectAccessor(new ObjectAccessor<T>());
    }

    public static ObjectAccessor<T> AddObjectAccessor<T>(this IServiceCollection services, T obj)
    {
        return services.AddObjectAccessor(new ObjectAccessor<T>(obj));
    }

    public static ObjectAccessor<T> AddObjectAccessor<T>(this IServiceCollection services, ObjectAccessor<T> accessor)
    {
        if (services.Any(s => s.ServiceType == typeof(ObjectAccessor<T>)))
        {
            throw new ApplicationException("An object accessor is registered before for type: " + typeof(T).AssemblyQualifiedName);
        }

        //Add to the beginning for fast retrieve
        services.Insert(0, ServiceDescriptor.Singleton(typeof(ObjectAccessor<T>), accessor));
        services.Insert(0, ServiceDescriptor.Singleton(typeof(IObjectAccessor<T>), accessor));

        return accessor;
    }
}