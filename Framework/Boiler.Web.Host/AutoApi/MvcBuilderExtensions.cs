using Boiler.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Options;

namespace Boiler.Web.Host.AutoApi;

public static class MvcBuilderExtensions
{
    public static IMvcBuilder AddConventionalControllers(this IMvcBuilder mvcBuilder)
    {
        mvcBuilder.Services.AddSingleton<IConfigureOptions<MvcOptions>, ConfigureAutoApiMvcOptions>();
        mvcBuilder.Services.AddTransient<AppServiceControllerConvention>();
        mvcBuilder.Services.AddTransient<AppServiceActionConvention>();
        mvcBuilder.Services.AddTransient<ConflictResolutionConvention>();
        mvcBuilder.Services.AddTransient<IApiDescriptionProvider, ApiDescriptionProvider>();

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
            });
        // .AddMvcOptions(options =>
        // {
        //     options.Conventions.Add(new AppServiceControllerConvention());
        //     options.Conventions.Add(new AppServiceActionConvention());
        //     options.Conventions.Add(new ConflictResolutionConvention());
        // });
    }
    
    private class ConfigureAutoApiMvcOptions: IConfigureOptions<MvcOptions>, ISingletonDependency
    {
        private readonly IServiceProvider _serviceProvider;

        public ConfigureAutoApiMvcOptions(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Configure(MvcOptions options)
        {
            options.Conventions.Add(_serviceProvider.GetService<AppServiceControllerConvention>());
            options.Conventions.Add(_serviceProvider.GetService<AppServiceActionConvention>());
            options.Conventions.Add(_serviceProvider.GetService<ConflictResolutionConvention>());
        }
    }
}