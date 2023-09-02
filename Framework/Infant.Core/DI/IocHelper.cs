using System.Reflection;
using Autofac;
using Autofac.Builder;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using Infant.Core.Modularity;
using Infant.Core.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Infant.Core.DI;

public static class IocHelper
{
    public static readonly Dictionary<Type, ServiceLifetime> DependencyLifetimeMapping =
        new Dictionary<Type, ServiceLifetime>(3)
        {
            [typeof(ITransientDependency)] = ServiceLifetime.Transient,
            [typeof(ISingletonDependency)] = ServiceLifetime.Singleton,
            [typeof(IScopedDependency)] = ServiceLifetime.Scoped
        };

    private static readonly HashSet<Type> _interceptors = new HashSet<Type>();

    public static void RegisterConventionalDependenciesFromAssembly(IServiceCollection serviceCollection, Assembly assembly,
        bool resolveBySelf = true,
        bool registerInterceptors = true)
    {
        RegisterInterceptor(serviceCollection, typeof(LogInterceptor));

        var containerBuilder = serviceCollection.GetSingletonInstance<ContainerBuilder>();
        var dependencyTypes = DependencyLifetimeMapping.Keys.ToArray();
        foreach (var dependencyType in dependencyTypes)
        {
            var lifetime = DependencyLifetimeMapping[dependencyType];
            var all = new AssemblyScanner(assembly).FindTypesByInterface(dependencyType);

            var baseTypes = all.Where(t => IsNonConcreteClassType(t, dependencyType)).ToList();

            var implementationTypes = all.Where(IsConcreteClassType).ToList();

            foreach (var implementation in implementationTypes)
            {
                // register concrete class itself
                var registrationBuilder = containerBuilder.RegisterType(implementation).SetAutofacLifetime(lifetime);
                var depTypes = new List<Type>(8);

                depTypes.AddRange(baseTypes.Where(nc => implementation.IsAssignableTo(nc)).ToArray());

                if (resolveBySelf)
                {
                    depTypes.Add(implementation);
                }

                registrationBuilder = registrationBuilder.As(depTypes.ToArray());
                registrationBuilder
                    // .EnableInterfaceInterceptors()
                    .EnableClassInterceptors()
                    .InterceptedBy(typeof(LogInterceptor));
            }
        }
    }

    private static void RegisterInterceptor(IServiceCollection serviceCollection, Type interceptor)
    {
        if (!interceptor.IsAssignableTo<IAsyncInterceptor>())
        {
            throw new Exception($"Type [{interceptor.FullName}] does not implement [{nameof(IAsyncInterceptor)}].");
        }

        if (!serviceCollection.Any(s => s.ServiceType == interceptor))
        {
            serviceCollection.AddTransient<LogInterceptor>();
        }

        _interceptors.Add(interceptor);
    }

    private static bool IsConcreteClassType(Type t)
    {
        return t.IsClass && !t.IsAbstract;
    }

    private static bool IsNonConcreteClassType(Type t, Type dependencyType)
    {
        return t.IsAbstract || (t.IsInterface && t != dependencyType);
    }

    private static IRegistrationBuilder<T1, T2, T3> SetAutofacLifetime<T1, T2, T3>(this IRegistrationBuilder<T1, T2, T3> registrationBuilder, ServiceLifetime lifetime)
    {
        switch (lifetime)
        {
            case ServiceLifetime.Singleton:
                return registrationBuilder.SingleInstance();
            case ServiceLifetime.Scoped:
                return registrationBuilder.InstancePerLifetimeScope();
            case ServiceLifetime.Transient:
                return registrationBuilder.InstancePerDependency();
            default:
                return registrationBuilder;
        }
    }
}