using System.Reflection;
using Autofac;
using Castle.DynamicProxy;
using Infant.Core.Ioc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;

namespace Infant.Core.Modularity;

public class AppModuleManager
{
    private readonly List<Type> _modules = new List<Type>();
    private List<Assembly> _assemblies = new List<Assembly>();
    private readonly List<InterceptorRegistration> _interceptorRegistrations = new List<InterceptorRegistration>();

    private readonly Type _rootAppModuleType;

    private readonly IServiceCollection _serviceCollection;

    public AppModuleManager(IServiceCollection serviceCollection, Type rootAppModuleType)
    {
        _serviceCollection = serviceCollection;
        _rootAppModuleType = rootAppModuleType;
        serviceCollection.GetSingletonInstance<ContainerBuilder>().RegisterInstance(this).As<AppModuleManager>();
        var interceptors = serviceCollection.GetSingletonInstanceOrNull<WebApplicationSettings>()?.Interceptors;
        if (interceptors is not null)
        {
            foreach (var interceptor in interceptors)
            {
                RegisterInterceptor(interceptor);
            }
        }
    }

    #region Methods

    internal void RegisterInterceptor(Type interceptor)
    {
        if (!interceptor.IsAssignableTo(typeof(IAsyncInterceptor)))
        {
            throw new ArgumentException($"Interceptor [${interceptor.FullName}] must implement [${nameof(IAsyncInterceptor)}]");
        }

        if (!_interceptorRegistrations.Any(r => r.Interceptor == interceptor))
        {
            _interceptorRegistrations.Add(new InterceptorRegistration
            {
                Interceptor = interceptor,
                TargetTypes = interceptor.GetCustomAttributes<InterceptsAttribute>()
                    .SelectMany(i => i.ModuleTypes)
                    .Distinct()
                    .ToArray()
            });
        }
        
        if (!_serviceCollection.Any(s => s.ServiceType == interceptor))
        {
            _serviceCollection.AddTransient(interceptor);
        }
    }

    internal Type[] GetInterceptorsForTargetTypes(IEnumerable<Type> targets)
    {
        return _interceptorRegistrations.Where(ir => targets.Any(t => ir.TargetTypes.Contains(t)))
            .Select(ir => ir.Interceptor)
            .ToArray();
    }

    public List<Assembly> GetInitializedAssemblies()
    {
        return _assemblies;
    }

    public void RegisterApplicationModules(IConfiguration configuration)
    {
        // var impl = ConfigurationHelper.BuildConfiguration(configOptions ?? new ConfigOptions());
        _serviceCollection.Replace(ServiceDescriptor.Singleton<IConfiguration>(configuration));

        RegisterModuleAndDependenciesRecursively(_rootAppModuleType);
    }

    #endregion

    #region Private Methods

    private void RegisterModuleAndDependenciesRecursively(Type type)
    {
        if (_modules.Contains(type))
        {
            return;
        }

        _modules.Add(type);

        var dependencyTypes = type.GetCustomAttributes<DependsOnAttribute>()
            .SelectMany(doa => doa.ModuleTypes).ToHashSet();
        if (dependencyTypes.Any())
        {
            foreach (var dependencyType in dependencyTypes)
            {
                RegisterModuleAndDependenciesRecursively(dependencyType);
            }
        }

        InitializeModule(type);
    }

    private void AutoRegisterModuleDependenciesFromAssembly(Type module, bool resolveBySelf = true)
    {
        if (_assemblies.Contains(module.Assembly)) return;

        IocHelper.RegisterConventionalDependenciesFromAssembly(_serviceCollection, module.Assembly, resolveBySelf);
        _assemblies.Add(module.Assembly);
    }

    private void InitializeModule(Type type)
    {
        var moduleInstance = Activator.CreateInstance(type) as AppModule;

        if (moduleInstance!.AutoRegisterDependencies)
        {
            AutoRegisterModuleDependenciesFromAssembly(type);
        }

        _serviceCollection.AddSingleton<AppModule>(provider => moduleInstance);
        moduleInstance.ConfigureServices(_serviceCollection);
    }

    #endregion
}