using System.Linq;
using Boiler.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;

namespace Infant.Host.AutoApi;

public class ActionProps
{
    public bool Conventional = true;
}

public class ApiDescProvider: IApiDescriptionProvider
{
    public void OnProvidersExecuting(ApiDescriptionProviderContext context)
    {
        // throw new NotImplementedException();
    }

    public void OnProvidersExecuted(ApiDescriptionProviderContext context)
    {
        foreach (var apiDescription in context.Results)
        {
            if (apiDescription.HttpMethod == null)
            {
                var methods = apiDescription.ActionDescriptor.EndpointMetadata.Where(em => em is HttpMethodMetadata);
                apiDescription.HttpMethod = (methods.FirstOrDefault() as HttpMethodMetadata)?.HttpMethods?[0];
            }
            
        }
        
    }

    public int Order { get; }
}

public class AppServiceControllerConvention : IControllerModelConvention
{
    public void Apply(ControllerModel controller)
    {
        if (typeof(IAppService).IsAssignableFrom(controller.ControllerType))
        {
            // Apply [ApiController] and default route
            controller.Filters.Add(new ApiControllerAttribute());
            controller.Selectors.Add(new SelectorModel
            {
                AttributeRouteModel = new AttributeRouteModel(
                    // new RouteAttribute($"api/{controller.ControllerName.Replace("AppService", "").Replace("Service", "")}/[action]")
                    new RouteAttribute($"api/{CreateRouteFromName(controller.ControllerName)}")
                    {
                        // Name = controller.ControllerName.Replace("AppService", "").Replace("Service", "")
                    })
                {
                    // Name = controller.ControllerName.Replace("AppService", "").Replace("Service", "")
                }
            });
            // Inject controller properties from services
            foreach (var controllerControllerProperty in controller.ControllerProperties)
            {
                var propType = controllerControllerProperty.PropertyInfo.PropertyType;
                if (propType.IsInterface || propType.IsAbstract)
                {
                    controllerControllerProperty.BindingInfo = new BindingInfo
                    {
                        BindingSource = BindingSource.Services
                    };
                }
            }
        }
    }

    private string CreateRouteFromName(string name)
    {
        return ToKebabCase(name.Replace("AppService", "").Replace("Service", ""));
    }
    
    private static string ToKebabCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        return string.Concat(input.Select((c, i) =>
            i > 0 && char.IsUpper(c) ? "-" + c.ToString() : c.ToString())).ToLower();
    }
}