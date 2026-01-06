using Boiler.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Options;

namespace Boiler.Web.Host.AutoApi;

public class AppServiceActionConvention : IActionModelConvention
{
    private readonly AutoApiOptions _options;

    private static readonly Dictionary<string[], Tuple<string, Type>> HttpVerbAttributes = new()
    {
        { ["Get", "GetAll", "Find", "List"], new("GET", typeof(HttpGetAttribute)) },
        { ["Create", "Post", "Update", "Patch", "Add"], new("POST", typeof(HttpGetAttribute)) },
        { ["Delete", "Remove"], new("DELETE", typeof(HttpGetAttribute)) }
    };

    public AppServiceActionConvention(IOptions<AutoApiOptions> options)
    {
        _options = options.Value ?? new AutoApiOptions();
    }

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

        if (!_options.IsEnabled)
            return;

        var methodName = action.ActionMethod.Name;

        foreach (var (prefixes, httpMethod) in HttpVerbAttributes)
        {
            // pick the longest matching prefix to avoid "Get" matching before "GetAll"
            var matchedPrefix = prefixes
                .OrderByDescending(p => p.Length)
                .FirstOrDefault(p => methodName.StartsWith(p, StringComparison.OrdinalIgnoreCase));

            if (matchedPrefix is null)
                continue;

            var routeSegment = BuildRouteSegment(action, methodName, matchedPrefix);

            if (!string.IsNullOrEmpty(matchedPrefix))
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
                        Template = routeSegment, // empty => clean root
                    }
                });
            }

            break;
        }
    }

    private string BuildRouteSegment(ActionModel action, string methodName, string matchedPrefix)
    {
        // FullMethodName: always kebab of full method
        if (_options.RouteStyle == AutoApiRouteStyle.FullMethodName)
            return methodName.ToKebabCase();

        // StripPrefix / RestfulCleanRoot: remove prefix and work with the remainder
        var remainder = methodName.Substring(matchedPrefix.Length);

        // "pure verb" methods => root (optional)
        if (string.IsNullOrWhiteSpace(remainder) && _options.UseRootForPureVerbActions)
            return string.Empty;

        // Turn remainder into a route segment
        var remainderKebab = remainder.ToKebabCase();

        if (_options.RouteStyle == AutoApiRouteStyle.RestfulCleanRoot && _options.AvoidEntityDuplicateInSubRoute)
        {
            // Detect entity name from controller (best-effort, no new dependencies)
            var controllerName = action.Controller.ControllerName ?? string.Empty;

            // common patterns: UsersAppService / UsersApplicationService / UsersController
            controllerName = controllerName
                .Replace("AppService", "", StringComparison.OrdinalIgnoreCase)
                .Replace("ApplicationService", "", StringComparison.OrdinalIgnoreCase)
                .Replace("Controller", "", StringComparison.OrdinalIgnoreCase);

            var entityKebab = controllerName.ToKebabCase();

            // If method remainder is exactly the entity name => root (avoid /users/users)
            if (!string.IsNullOrEmpty(entityKebab) &&
                string.Equals(remainderKebab, entityKebab, StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }
        }

        return remainderKebab;
    }
}
