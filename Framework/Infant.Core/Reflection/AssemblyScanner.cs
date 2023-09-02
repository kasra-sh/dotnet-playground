using System.Reflection;

namespace Infant.Core.Reflection;

public class AssemblyScanner
{
    private readonly Assembly _assembly;

    public AssemblyScanner(Assembly assembly)
    {
        _assembly = assembly;
    }

    public ICollection<Type> FindTypesByInterface(Type type)
    {
        return _assembly.GetExportedTypes().Where(t => t.IsAssignableTo(type) && t != type).ToArray();
    }
}