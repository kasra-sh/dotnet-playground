using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Boiler.Web.Host.AutoApi;

public class ApiDescriptionProvider : IApiDescriptionProvider
{
    public void OnProvidersExecuting(ApiDescriptionProviderContext context) { }

    public void OnProvidersExecuted(ApiDescriptionProviderContext context)
    {
        foreach (var apiDescription in context.Results)
        {
            if (apiDescription.HttpMethod == null)
            {
                var methodMetadata = apiDescription.ActionDescriptor.EndpointMetadata
                    .OfType<HttpMethodMetadata>()
                    .FirstOrDefault();

                if (methodMetadata?.HttpMethods is { Count: > 0 })
                {
                    apiDescription.HttpMethod = methodMetadata.HttpMethods[0];
                }
            }
        }
    }

    public int Order => 100; // Run after default providers
}