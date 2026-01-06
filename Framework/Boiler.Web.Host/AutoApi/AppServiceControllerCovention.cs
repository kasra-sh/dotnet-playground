using Boiler.Core.Abstractions;
using Boiler.Core.Modularity.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Boiler.Web.Host.AutoApi;

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
            var path = "api/";
            var version = controller.Attributes.OfType<ApiVersionAttribute>().FirstOrDefault();
            if (version != null)
            {
                path += $"v{version.VersionString}";
            }
            path += $"/{CreateRouteFromName(controller.ControllerName)}";
            
            controller.Selectors.Add(new SelectorModel
            {
                AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(path))
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