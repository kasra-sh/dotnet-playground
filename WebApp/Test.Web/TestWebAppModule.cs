using Boiler.Core;
using Boiler.Core.Modularity;
using Boiler.Core.Modularity.Attributes;

namespace Test.Web;

[DependsOn(typeof(BoilerCoreModule))]
public class TestWebAppModule: AppModule
{
    
}