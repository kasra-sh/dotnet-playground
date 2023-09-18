namespace Infant.Core.Ioc;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class InterceptsAttribute: Attribute
{
    public List<Type> ModuleTypes { get; }
    
    public InterceptsAttribute(params Type[] targetTypes)
    {
        ModuleTypes = targetTypes.ToList();
    }

    public InterceptsAttribute(Type moduleType)
    {
        ModuleTypes = new List<Type>(1){ moduleType };
    }
}