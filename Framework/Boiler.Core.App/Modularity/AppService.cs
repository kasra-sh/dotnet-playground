using System;
using System.Security.Claims;
using Boiler.Core.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Boiler.Core.Modularity;

public abstract class AppService: IAppService
{
    public required IServiceProvider ServiceProvider { get; set; }
    public required IHttpContextAccessor Accessor { get; set; }
    
    public AppService()
    {
        
    }
    
    protected ClaimsPrincipal CurrentUser
    {
        get
        {
            if (ServiceProvider == null)
            {
                Log.Warning("ServiceProvider is not injected by property in AppService!");
            }
            return ServiceProvider.GetService<IHttpContextAccessor>()?.HttpContext?.User;
        }
    }

}