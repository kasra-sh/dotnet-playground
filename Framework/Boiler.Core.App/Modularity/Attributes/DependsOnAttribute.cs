using System;
using System.Collections.Generic;
using System.Linq;

namespace Boiler.Core.Modularity.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DependsOnAttribute: Attribute
{
    public List<Type> ModuleTypes { get; }
    
    public DependsOnAttribute(params Type[] moduleTypes)
    {
        ModuleTypes = moduleTypes.ToList();
    }

    public DependsOnAttribute(Type moduleType)
    {
        ModuleTypes = new List<Type>(1){ moduleType };
    }
}