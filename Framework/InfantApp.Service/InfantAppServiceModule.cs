using Infant.Core.Modularity;
using InfantApp.Domain;

namespace InfantApp.Service;

[DependsOn(typeof(InfantAppDomainModule))]
public class InfantAppServiceModule: AppModule
{
}