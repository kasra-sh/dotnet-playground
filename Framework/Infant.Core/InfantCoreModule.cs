using Infant.Core.Modularity;
using Microsoft.Extensions.DependencyInjection;

namespace Infant.Core;

public class InfantCoreModule: AppModule
{
    public override void ConfigureServices(IServiceCollection services)
    {
        // services.AddSingleton(typeof(AsyncLocalObjectAccessor<>), typeof(AsyncLocalObjectAccessor<>));
    }
}