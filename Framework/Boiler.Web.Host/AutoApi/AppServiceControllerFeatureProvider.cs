using System.Reflection;
using Boiler.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Boiler.Web.Host.AutoApi;

public class AppServiceControllerFeatureProvider : ControllerFeatureProvider
{
    protected override bool IsController(TypeInfo typeInfo)
    {
        return typeof(IAppService).IsAssignableFrom(typeInfo) &&
               typeInfo is { IsAbstract: false, IsClass: true, ContainsGenericParameters: false } &&
               !typeInfo.IsDefined(typeof(NonControllerAttribute), inherit: true);
    }
}