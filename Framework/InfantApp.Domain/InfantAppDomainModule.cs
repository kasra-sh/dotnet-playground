using Infant.Core.Modularity;
using Microsoft.Extensions.DependencyInjection;

namespace InfantApp.Domain;

public class InfantAppDomainModule: AppModule
{
    public override async Task ConfigureServicesAsync(IServiceCollection services)
    {
        // services.AddTransient<ProductManager>();
    }

    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
    }
}