using Boiler.Core.Abstractions;
using Boiler.Core.Modularity.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Boiler.Web.Host.AutoApi;

public class AppServiceControllerConvention : IControllerModelConvention
{
    public void Apply(ControllerModel controller)
    {
        if (typeof(IAppService).IsAssignableFrom(controller.ControllerType))
        {
            controller.Filters.Add(new ApiControllerAttribute());

            // Mark as auto-generated for conflict resolution
            controller.Properties["IsAutoApiController"] = true;

            var path = "api/";
            var version = controller.Attributes.OfType<ApiVersionAttribute>().FirstOrDefault();
            if (version != null)
            {
                path += $"v{version.VersionString}/";
            }

            path += CreateRouteFromName(controller.ControllerName);

            controller.Selectors.Add(new SelectorModel
            {
                AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(path))
            });

            // Inject controller properties from DI if they are interfaces/abstract
            foreach (var property in controller.ControllerProperties)
            {
                var propType = property.PropertyInfo.PropertyType;
                if (propType.IsInterface || propType.IsAbstract)
                {
                    property.BindingInfo ??= new BindingInfo();
                    property.BindingInfo.BindingSource = BindingSource.Services;
                }
            }
        }
    }

    private static string CreateRouteFromName(string name)
    {
        return (name.Replace("AppService", "").Replace("Service", "")).ToKebabCase();
    }
}