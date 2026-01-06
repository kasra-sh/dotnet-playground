using Boiler.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Boiler.Web.Host.AutoApi;

public class AppServiceActionConvention : IActionModelConvention
{
    private static readonly Dictionary<string[], Tuple<string, Type>> HttpVerbAttributes = new()
    {
        { ["Get", "GetAll", "Find", "List"], new("GET", typeof(HttpGetAttribute)) },
        { ["Create", "Post", "Update", "Patch", "Add"], new("POST", typeof(HttpGetAttribute)) },
        { ["Delete", "Remove"], new("DELETE", typeof(HttpGetAttribute)) }
    };

    public void Apply(ActionModel action)
    {
        if (!typeof(IAppService).IsAssignableFrom(action.Controller.ControllerType))
            return;
        foreach (var parameter in action.Parameters)
        {
            if (parameter.ParameterType.IsInterface || !parameter.ParameterType.IsClass ||
                parameter.ParameterType.IsAbstract)
            {
                parameter.BindingInfo = new BindingInfo
                {
                    BindingSource = BindingSource.Special,
                    BinderType = typeof(ServicesModelBinder)
                };
            }
        }

        var methodName = action.ActionMethod.Name;
        foreach (var (prefix, httpMethod) in HttpVerbAttributes)
        {
            if (prefix.Any(p => methodName.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
            {
                // Create the appropriate HTTP verb attribute
                // Set the route template if needed
                var routeSegment = ToKebabCase(methodName);
                if (!string.IsNullOrEmpty(routeSegment))
                {
                    var httpVerbAttribute =
                        (Attribute)Activator.CreateInstance(httpMethod.Item2, new object[] { routeSegment })!;
                    ((List<object>)action.Attributes).Add(httpVerbAttribute);
                    action.RouteValues.Add("action", routeSegment);
                    action.RouteValues.Add("route", routeSegment);

                    action.Selectors[0].EndpointMetadata.Add(httpVerbAttribute);
                    action.Selectors[0].EndpointMetadata.Add(new HttpMethodMetadata([httpMethod.Item1]));
                    action.Selectors[0].AttributeRouteModel = new AttributeRouteModel
                    {
                        Template = routeSegment,
                    };
                }
                else
                {
                    var httpVerbAttribute = (Attribute)Activator.CreateInstance(httpMethod.Item2)!;

                    action.Selectors.Add(new SelectorModel
                    {
                        EndpointMetadata = { httpVerbAttribute },
                        AttributeRouteModel = new AttributeRouteModel
                        {
                            Template = routeSegment,
                        }
                    });
                }

                break;
            }
        }
    }

    private static string ToKebabCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        return string.Concat(input.Select((c, i) =>
            i > 0 && char.IsUpper(c) ? "-" + c.ToString() : c.ToString())).ToLower();
    }
}