using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Infant.Core.Modularity;

public class AutofacServiceProviderFactory : IServiceProviderFactory<ContainerBuilder>
{
    private readonly ContainerBuilder _builder;
    private IServiceCollection _services;

    public AutofacServiceProviderFactory(ContainerBuilder builder)
    {
        _builder = builder;
    }
    
    public ContainerBuilder CreateBuilder(IServiceCollection services)
    {
        _services = services;
        return _builder;
    }

    public IServiceProvider CreateServiceProvider(ContainerBuilder containerBuilder)
    {
        _builder.Populate(_services);
        return new AutofacServiceProvider(containerBuilder.Build());
    }
}
