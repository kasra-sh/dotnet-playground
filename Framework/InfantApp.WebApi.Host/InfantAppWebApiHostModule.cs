using Infant.Core;
using Infant.Core.Modularity;
using Infant.Host;
using InfantApp.Ef;
using InfantApp.Service;

namespace InfantApp.WebApi.Host;

[DependsOn(
    typeof(InfantCoreModule),
    typeof(InfantAppServiceModule),
    typeof(InfantAppEfModule),
    typeof(InfantHostWebModule)
)]
public class InfantAppWebApiHostModule : AppModule
{
    public override void ConfigureServices(IServiceCollection services)
    {
    }
}