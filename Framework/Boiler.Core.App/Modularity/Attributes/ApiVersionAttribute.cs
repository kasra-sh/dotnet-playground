using System;

namespace Boiler.Core.Modularity.Attributes;

public class ApiVersionAttribute: Attribute
{
    public string VersionString { get; }

    public ApiVersionAttribute(int major)
    {
        VersionString = $"{major}";
    }
    
}