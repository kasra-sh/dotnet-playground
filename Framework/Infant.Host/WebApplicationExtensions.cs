using Autofac;
using Autofac.Extensions.DependencyInjection;
using Infant.Core.Modularity;
using AutofacServiceProviderFactory = Infant.Core.Modularity.AutofacServiceProviderFactory;

// using Microsoft.AspNetCore.Builder;

namespace Infant.Host;

public static class WebApplicationExtensions
{
    public static Task<WebApplication> InitializeApplicationAsync(this WebApplication webApplication)
    {
        // call lifecycle methods
        return Task.FromResult(webApplication);
    }

    public static async Task AddApplicationAsync<TModule>(
        this WebApplicationBuilder webAppBuilder)
    {
        var containerBuilder = new ContainerBuilder();
        webAppBuilder.Services.AddSingleton(containerBuilder);
        webAppBuilder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory(containerBuilder));
            // .ConfigureServices((context, services) =>
            // {
            // });
        webAppBuilder.Services.AddApplicationModule<TModule>();
        
        webAppBuilder.Services.AddControllers();
        
        webAppBuilder.Services.AddEndpointsApiExplorer();
        webAppBuilder.Services.AddSwaggerGen();
    }
}