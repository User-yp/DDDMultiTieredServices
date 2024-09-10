using FluentValidation;
using MTS.Domain;
using MTS.Domain.IMiddleResp;
using MTS.Domain.IRepository;
using MTS.Infrastructure.MiddleResp;
using MTS.Infrastructure.Repository;
using Newtonsoft.Json;
using System.Collections.Frozen;
using System.Reflection;

namespace MTS.WebApi;

public static class ServiceExtensions
{
    public static void AddBaseServies(this IServiceCollection services)
    {
        services.AddScoped<DomainService>();

        services.AddScoped<IModelMiddleResp, ModelMiddleResp>();
        services.AddScoped<IOrderMiddleResp, OrderMiddleResp>();

        services.AddScoped<IModelRepository, ModelRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
    }
    public static void AddValidatorsFromAssembly(this IServiceCollection services, IEnumerable<Assembly> assemblies)
    {
        var validatorDictionary = GetAbstractValidatorClasses(assemblies);
        foreach (var kvp in validatorDictionary)
        {
            var validatorType = kvp.Key;
            var modelType = kvp.Value;

            //services.AddScoped<typeof(IValidator<>).MakeGenericType(modelType) ,validatorType > ();

            //AddScoped<IValidator<AddOrderRequset>, AddOrderRequsetValidator>();
            var addScopedGenericMethod = typeof(ServiceCollectionServiceExtensions)
                .GetMethod("AddScoped", [typeof(IServiceCollection)])
                .MakeGenericMethod(typeof(IValidator<>).MakeGenericType(modelType), validatorType);

            // 调用 AddScoped 方法  
            addScopedGenericMethod.Invoke(null, [services]);
        }
    }
    public static FrozenDictionary<Type, Type> GetAbstractValidatorClasses(IEnumerable<Assembly> assemblies)
    {
        var validatorDictionary = new Dictionary<Type, Type>();

        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes()
                                .Where(type => type.IsClass &&
                                               !type.IsAbstract &&
                                               type.BaseType != null &&
                                               type.BaseType.IsGenericType &&
                                               type.BaseType.GetGenericTypeDefinition() == typeof(AbstractValidator<>) &&
                                               !IsInlineValidator(type)); // 排除 InlineValidator  

            foreach (var type in types)
            {
                var genericType = type.BaseType?.GetGenericArguments().FirstOrDefault();
                if (genericType != null&& genericType.FullName!=null)
                {
                    validatorDictionary[type] = genericType;
                }
            }
        }
        return validatorDictionary.ToFrozenDictionary();
    }
    private static bool IsInlineValidator(Type type)
    {
        // 检查是否是 InlineValidator<T> 的子类  
        return type.BaseType != null &&
               type.BaseType.IsGenericType &&
               type.BaseType.GetGenericTypeDefinition() == typeof(InlineValidator<>);
    }
    // 辅助方法：获取原始泛型类型  
    public static Type GetSubclassOfRawGeneric(this Type toCheck, Type generic)
    {
        while (toCheck != null && toCheck != typeof(object))
        {
            var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
            if (generic == cur)
            {
                return toCheck;
            }
            toCheck = toCheck.BaseType;
        }
        return null;
    }
}
