using System.Reflection;
using Autofac;
using Infant.Core.DI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Infant.Core.Modularity;

public class ApplicationManager
{
    private readonly List<Type> _initializedModules = new List<Type>();
    private List<Assembly> _initializedAssemblies = new List<Assembly>();
    private readonly Type _applicationModuleType;

    private readonly IServiceCollection _serviceCollection;

    public ApplicationManager(IServiceCollection serviceCollection, Type applicationModuleType)
    {
        _serviceCollection = serviceCollection;
        _applicationModuleType = applicationModuleType;
        serviceCollection.GetSingletonInstance<ContainerBuilder>().RegisterInstance(this).As<ApplicationManager>();
    }

    #region Methods

    public void RegisterApplicationModules(IConfiguration configuration)
    {
        // var impl = ConfigurationHelper.BuildConfiguration(configOptions ?? new ConfigOptions());
        _serviceCollection.Replace(ServiceDescriptor.Singleton<IConfiguration>(configuration));

        RegisterModuleAndDependenciesRecursively(_applicationModuleType);
    }

    #endregion

    #region Private Methods

    private void RegisterModuleAndDependenciesRecursively(Type type)
    {
        if (_initializedModules.Contains(type))
        {
            return;
        }

        var dependencyTypes = type.GetCustomAttributes<DependsOnAttribute>()
            .SelectMany(doa => doa.ModuleTypes).ToList();
        if (dependencyTypes.Any())
        {
            foreach (var dependencyType in dependencyTypes)
            {
                RegisterModuleAndDependenciesRecursively(dependencyType);
            }
        }

        InitializeModule(type);

        _initializedModules.Add(type);
    }

    private void AutoRegisterModuleDependenciesFromAssembly(Type module, bool resolveBySelf = true)
    {
        if (_initializedAssemblies.Contains(module.Assembly)) return;

        IocHelper.RegisterConventionalDependenciesFromAssembly(_serviceCollection, module.Assembly, resolveBySelf);
        _initializedAssemblies.Add(module.Assembly);
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