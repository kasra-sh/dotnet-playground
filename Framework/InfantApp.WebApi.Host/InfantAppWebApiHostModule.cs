using Infant.Core.Modularity;
using InfantApp.Service;

namespace InfantApp.WebApi.Host;

[DependsOn(typeof(InfantAppServiceModule))]
public class InfantAppWebApiHostModule: AppModule
{
    public override void ConfigureServices(IServiceCollection services)
    {
        
    }
}