using System.Security.Claims;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Runtime. CompilerServices;
using Infant.Core.Ioc;

namespace Infant.Core.Abstractions;

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