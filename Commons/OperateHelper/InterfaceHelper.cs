using System.Reflection;

namespace Commons.OperateHelper;

public static class InterfaceHelper
{
    public static List<Type> GetAttributeTypes<TAttribute>(IEnumerable<Assembly> assemblies) where TAttribute : class
    {
        List<Type> typeAttribute = assemblies
            .SelectMany(x => x.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract
                    && t.GetCustomAttributes(typeof(TAttribute), false).Length > 0)
            .ToList();

        return typeAttribute;
    }

    public static List<Type> GetAttributeTypes<TAttribute>(params Assembly[] assemblies) where TAttribute : class
    {
        List<Type> typeAttribute = assemblies
            .SelectMany(x => x.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract
                    && t.GetCustomAttributes(typeof(TAttribute), false).Length > 0)
            .ToList();

        return typeAttribute;
    }

    public static List<Type> GetInterfaceTypes<TInterface>(params Assembly[] assemblies) where TInterface : class
    {
        List<Type> typeInterface = assemblies
            .SelectMany(x => x.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract
                    && t.GetInterfaces().Contains(typeof(TInterface)))
            .ToList();

        return typeInterface;
    }

    public static List<Type> GetInterfaceTypes<TInterface>(IEnumerable<Assembly> assemblies) where TInterface : class
    {
        List<Type> typeInterface = assemblies
            .SelectMany(x => x.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract
                    && t.GetInterfaces().Contains(typeof(TInterface)))
            .ToList();

        return typeInterface;
    }
}
