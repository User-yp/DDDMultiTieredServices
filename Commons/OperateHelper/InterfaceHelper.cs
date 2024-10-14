using System.Reflection;

namespace Commons.OperateHelper;

public static class InterfaceHelper
{
    public static List<Type> GetAttributeTypes<TAttribute>(IEnumerable<Assembly> assemblies) where TAttribute : class
    {
        return GetAttributeTypes<TAttribute>(assemblies?.ToArray()??Array.Empty<Assembly>());
    }

    public static List<Type> GetAttributeTypes<TAttribute>(params Assembly[] assemblies) where TAttribute : class
    {
        if (assemblies == null || assemblies.Length == 0)
        {
            throw new ArgumentException("Assemblies cannot be null or empty");
        }

        List<Type> typeAttribute = assemblies
            .SelectMany(x => x.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract
                    && t.GetCustomAttributes(typeof(TAttribute), false).Length > 0)
            .ToList();

        return typeAttribute;
    }

    public static List<Type> GetInterfaceTypes<TInterface>(IEnumerable<Assembly> assemblies) where TInterface : class
    {
        return GetInterfaceTypes<TInterface>(assemblies?.ToArray() ?? Array.Empty<Assembly>());
    }

    public static List<Type> GetInterfaceTypes<TInterface>(params Assembly[] assemblies) where TInterface : class
    {
        if (assemblies == null || assemblies.Length == 0)
        {
            throw new ArgumentException("Assemblies cannot be null or empty");
        }

        List<Type> typeInterface = assemblies
            .SelectMany(x => x.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract
                    && t.GetInterfaces().Contains(typeof(TInterface)))
            .ToList();

        return typeInterface;
    }
}
