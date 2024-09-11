using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Validation;

[AttributeUsage(AttributeTargets.Class)]
public class ValidatorAttribute : Attribute
{
    public ServiceLifetime LifeTime { get; set; }
    public Type ServiceType { get; init; }
    public ValidatorAttribute(Type ServiceType, ServiceLifetime LifeTime = ServiceLifetime.Scoped)
    {
        //验证ServiceType是否继承自AbstractValidator<>
        if (!CheckValidator(ServiceType))
            throw new ArgumentException($"The type {ServiceType.FullName} must implement AbstractValidator<>.");

        this.LifeTime = LifeTime;
        this.ServiceType = ServiceType;
    }
    private static bool CheckValidator(Type type)
    {
        // 获取 AbstractValidator<T> 的类型  
        var abstractValidatorType = typeof(AbstractValidator<>);

        // 检查是否是 AbstractValidator<T> 的实现  
        if (type.IsGenericType && type.GetGenericTypeDefinition() == abstractValidatorType)
        {
            return true;
        }

        // 检查基类是否是 AbstractValidator<T>  
        var baseType = type.BaseType;
        while (baseType != null)
        {
            if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == abstractValidatorType)
            {
                return true;
            }
            baseType = baseType.BaseType;
        }

        // 检查实现的接口  
        return type.GetInterfaces().Any(i =>
            i.IsGenericType && i.GetGenericTypeDefinition() == abstractValidatorType);
    }
}
