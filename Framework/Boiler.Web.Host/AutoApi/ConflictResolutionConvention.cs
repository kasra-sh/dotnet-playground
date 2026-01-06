using Boiler.Core.Abstractions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Boiler.Web.Host.AutoApi;

public class ConflictResolutionConvention : IApplicationModelConvention, ITransientDependency
{
    public void Apply(ApplicationModel application)
    {
        var groups = new Dictionary<string, List<(ControllerModel Controller, ActionModel Action, SelectorModel Selector)>>();

        foreach (var controller in application.Controllers)
        {
            foreach (var action in controller.Actions)
            {
                foreach (var selector in action.Selectors)
                {
                    var routeModel = selector.AttributeRouteModel;
                    if (routeModel?.Template == null)
                        continue;

                    var template = routeModel.Template.Trim('/').ToLowerInvariant();

                    var methods = selector.EndpointMetadata
                        .OfType<HttpMethodMetadata>()
                        .SelectMany(m => m.HttpMethods ?? Enumerable.Empty<string>())
                        .Distinct()
                        .OrderBy(m => m)
                        .ToArray();

                    var methodsKey = methods.Length == 0 ? "NONE" : string.Join(",", methods);

                    var key = $"{methodsKey}:{template}";

                    if (!groups.TryGetValue(key, out var list))
                    {
                        list = new List<(ControllerModel, ActionModel, SelectorModel)>();
                        groups[key] = list;
                    }

                    list.Add((controller, action, selector));
                }
            }
        }

        foreach (var group in groups.Values)
        {
            if (group.Count <= 1)
                continue;

            // If there is at least one manual (non-auto) endpoint in the conflict group, remove all auto ones
            var hasManual = group.Any(e => !e.Controller.Properties.ContainsKey("IsAutoApiController"));
            if (hasManual)
            {
                var autoEntries = group.Where(e => e.Controller.Properties.ContainsKey("IsAutoApiController")).ToList();
                foreach (var entry in autoEntries)
                {
                    entry.Action.Selectors.Remove(entry.Selector);
                }
            }
            // If all are auto, leave them (ambiguity will be caught at startup if truly conflicting)
        }
    }
}