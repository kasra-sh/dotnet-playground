using Infant.Core;
using Infant.Core.Modularity;
using Infant.Host;
using InfantApp.Ef;
using InfantApp.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

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

    public override void OnApplicationInitialization(IServiceProvider services)
    {
    }
}