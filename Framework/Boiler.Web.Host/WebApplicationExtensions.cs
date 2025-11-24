using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Boiler.Core.Modularity;
using Boiler.Core.Modularity.Ioc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

// using Microsoft.AspNetCore.Builder;

namespace Infant.Host;

public static class WebApplicationExtensions
{
    public static void AddApplicationModule<TModule>(this WebApplicationBuilder webApplicationBuilder)
    {
        var applicationManager = new AppModuleManager(webApplicationBuilder.Services, typeof(TModule));
        webApplicationBuilder.Services.AddSingleton(applicationManager);
        applicationManager.RegisterApplicationModules(webApplicationBuilder.Configuration);
    }
    public static async Task AddApplicationAsync<TModule>(
        this WebApplicationBuilder webAppBuilder, WebApplicationSettings settings = null)
    {
        // replace SilentLogger with ConsoleLogger
        if (Log.Logger.GetType().Name == "SilentLogger")
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(theme: AnsiConsoleTheme.Sixteen)
                .CreateLogger();
        }

        if (settings is null)
        {
            settings = new WebApplicationSettings();
        }

        webAppBuilder.Services.AddSingleton(settings);

        webAppBuilder.Services.AddHttpContextAccessor();

        var containerBuilder = new ContainerBuilder();
        webAppBuilder.Services.AddSingleton(containerBuilder);
        webAppBuilder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory(containerBuilder));

        webAppBuilder.AddApplicationModule<TModule>();
    }

    public static async Task InitializeApplicationAsync(this WebApplication app)
    {
        var appModules = app.Services.GetServices<AppModule>().ToArray();

        foreach (var appModule in appModules)
        {
            if (CallAndCheckIsNotImplemented(() => appModule.OnApplicationInitialization(app.Services)))
            {
                await CallAndCheckIsNotImplementedAsync(async () =>
                    await appModule.OnApplicationInitializationAsync(app.Services));
            }
        }

        foreach (var appModule in appModules.Reverse())
        {
            app.Lifetime.ApplicationStopping.Register(() =>
            {
                CallAndCheckIsNotImplemented(() => appModule.OnApplicationShutdown(app.Services));
            });
        }
    }

    public static IServiceCollection AddDevelopmentSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        return services.AddSwaggerGen(c =>
        {
            // c.SwaggerDoc("v1", new Info { Title = "You api title", Version = "v1" });

            c.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme."
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "bearerAuth"
                        }
                    },
                    new string[] { }
                }
            });
            c.CustomOperationIds(apiDesc =>
            {
                var controllerName = apiDesc.ActionDescriptor.RouteValues["controller"];
                var actionName = apiDesc.ActionDescriptor.RouteValues["action"];
                return $"{controllerName}_{actionName}";
            });
            c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            c.TagActionsBy(apiDesc =>
            {
                var controller = apiDesc.ActionDescriptor.RouteValues["controller"].Replace("AppService", "");
                // var action = apiDesc.ActionDescriptor.RouteValues["action"];
                return new[] { $"{controller}" };
            });
            // c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            // {
            //     Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
            //               Enter 'Bearer' [space] and then your token in the text input below.
            //               \r\n\r\nExample: 'Bearer 12345abcdef'",
            //     Name = "Authorization",
            //     In = ParameterLocation.Header,
            //     Type = SecuritySchemeType.ApiKey,
            //     Scheme = "Bearer"
            // });

            // c.AddSecurityRequirement(new OpenApiSecurityRequirement()
            // {
            //     {
            //         new OpenApiSecurityScheme
            //         {
            //             Reference = new OpenApiReference
            //             {
            //                 Type = ReferenceType.SecurityScheme,
            //                 Id = "Bearer"
            //             },
            //             Scheme = "oauth2",
            //             Name = "Bearer",
            //             In = ParameterLocation.Header
            //         },
            //         new List<string> {/*"Kasra"*/}
            //     }
            // });
        });
    }

    private static bool CallAndCheckIsNotImplemented(Action action)
    {
        try
        {
            action.Invoke();
            return false;
        }
        catch (NotImplementedException)
        {
            return true;
        }
    }

    private static async Task<bool> CallAndCheckIsNotImplementedAsync(Func<Task> action)
    {
        try
        {
            await action.Invoke();
            return false;
        }
        catch (NotImplementedException)
        {
            return true;
        }
    }
}