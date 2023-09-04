using Infant.Core.Modularity;
using InfantApp.Ef;
using InfantApp.Service;

namespace InfantApp.WebApi.Host;

[DependsOn(
    typeof(InfantAppServiceModule),
    typeof(InfantAppEfModule)
)]
public class InfantAppWebApiHostModule : AppModule
{
    public override void ConfigureServices(IServiceCollection services)
    {
    }
}