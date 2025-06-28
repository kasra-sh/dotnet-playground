using Infant.Core.Ioc;
using Microsoft.AspNetCore.Http;

namespace Infant.Core.Abstractions;

public interface IAppService: ITransientDependency
{
    //
    // public IServiceProvider ServiceProvider { get; set; }
    // public IHttpContextAccessor Accessor { get; set; }
}