using Boiler.Core.Abstractions;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Infant.Host.AutoApi;

public static class MvcBuilderExtensions
{
    public static IMvcBuilder AddConventionalControllers(this IMvcBuilder mvcBuilder)
    {
        return mvcBuilder
            .ConfigureApplicationPartManager(manager =>
            {
                var types = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => (!a.FullName?.StartsWith("System") ?? false) && !a.FullName.StartsWith("Microsoft"))
                    .SelectMany(a => a.GetTypes()).ToArray();

                // Automatically register all IAppService implementations as ApiControllers
                var serviceTypes = types
                    .Where(t => typeof(IAppService).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);
                var assemblies = serviceTypes.Select(t => t.Assembly).ToList();
                foreach (var assembly in assemblies)
                {
                    manager.ApplicationParts.Add(new AssemblyPart(assembly));
                }

                manager.FeatureProviders.Add(new AppServiceControllerFeatureProvider());
            })
            .AddMvcOptions(options =>
            {
                options.Conventions.Add(new AppServiceControllerConvention());
                options.Conventions.Add(new AppServiceActionConvention());
            });
    }
}