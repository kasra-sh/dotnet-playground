using Infant.Core.Modularity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InfantApp.Ef;

public class InfantAppEfModule: AppModule
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<InfantDbContext>(options =>
        {
            options.UseSqlServer(services.GetConfiguration().GetConnectionString("InfantDb"));
            options.AddInterceptors();
        }, contextLifetime: ServiceLifetime.Scoped, optionsLifetime: ServiceLifetime.Singleton);
    }
}