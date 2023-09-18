using System.Reflection;
using Autofac;
using Autofac.Builder;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using Infant.Core.Modularity;
using Infant.Core.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Infant.Core.Ioc;

public static class IocHelper
{
    public static readonly Dictionary<Type, ServiceLifetime> DependencyLifetimeMapping =
        new Dictionary<Type, ServiceLifetime>(3)
        {
            [typeof(ITransientDependency)] = ServiceLifetime.Transient,
            [typeof(ISingletonDependency)] = ServiceLifetime.Singleton,
            [typeof(IScopedDependency)] = ServiceLifetime.Scoped
        };

    // private static readonly HashSet<Type> _interceptors = new HashSet<Type>();

    public static void RegisterConventionalDependenciesFromAssembly(IServiceCollection serviceCollection, Assembly assembly,
        bool resolveBySelf = true,
        bool registerInterceptors = true)
    {
        var appModuleManager = serviceCollection.GetSingletonInstance<AppModuleManager>();
        var containerBuilder = serviceCollection.GetSingletonInstance<ContainerBuilder>();
        var dependencyTypes = DependencyLifetimeMapping.Keys.ToArray();
        foreach (var dependencyType in dependencyTypes)
        {
            var lifetime = DependencyLifetimeMapping[dependencyType];
            var typesWithDependencyInterface = new AssemblyScanner(assembly).FindTypesByInterface(dependencyType);

            var implementationTypes = typesWithDependencyInterface.Where(IsConcreteClassType).ToList();

            var depTypes = new List<Type>(8);

            foreach (var implementation in implementationTypes)
            {
                var registrationBuilder = containerBuilder.RegisterType(implementation).SetAutofacLifetime(lifetime);
                depTypes.Clear();
                depTypes.AddRange(
                    implementation.GetInterfaces().Where(i => i != dependencyType && i.IsAssignableTo(dependencyType))
                );

                if (resolveBySelf)
                {
                    depTypes.Add(implementation);
                }

                registrationBuilder = registrationBuilder.As(depTypes.ToArray());
                if (registerInterceptors)
                {
                    var interceptors = appModuleManager.GetInterceptorsForTargetTypes(depTypes);
                    if (interceptors.Any())
                    {
                        registrationBuilder
                            .EnableClassInterceptors()
                            .InterceptedBy(interceptors);
                    }
                }
            }
        }
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