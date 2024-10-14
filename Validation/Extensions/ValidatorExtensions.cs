using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Validation.Attributes;

namespace Validation.Extensions;

public static class ValidatorExtensions
{
    /// <summary>
    /// 按特性中的生命周期注入业务组件
    /// </summary>
    /// <param name="service"></param>
    public static IServiceCollection AddFluentValidation(this IServiceCollection service, IEnumerable<Assembly> assemblies)
    {
        return service.AddFluentValidation(assemblies?.ToArray() ?? Array.Empty<Assembly>());
    }
    public static IServiceCollection AddFluentValidation(this IServiceCollection service, params Assembly[] assemblies)
    {
        var ser = service.InitValidatorService(assemblies);
        service.AddValidatorControl();
        return ser;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="service"></param>
    /// <param name="assemblies">指定程序集</param>
    /// <returns></returns>
    public static IServiceCollection InitValidatorService(this IServiceCollection service, params Assembly[] assemblies)
    {
        if (assemblies == null || assemblies.Length == 0)
        {
            throw new ArgumentException("Assemblies cannot be null or empty");
        }
        //获取有ServiceAttribute特性的所有类
        List<Type> typeAttribute = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract
                && t.GetCustomAttributes(typeof(ValidatorAttribute), false).Length != 0)
                .ToList();

        if (typeAttribute.Count == 0)
            return service;

        typeAttribute.ForEach(impl =>
        {
            //获取生命周期
            var lifetime = impl.GetCustomAttribute<ValidatorAttribute>().LifeTime;
            //获取ServiceType
            var serviceType = impl.GetCustomAttribute<ValidatorAttribute>().ServiceType;
            //写入泛型参数，获取IValidator<>类型
            var validatorType = typeof(IValidator<>).MakeGenericType(impl);

            //var res=typeof()
            //获取该类注入的生命周期
            switch (lifetime)
            {
                case ServiceLifetime.Singleton:
                    service.AddSingleton(validatorType, serviceType);
                    break;
                case ServiceLifetime.Scoped:
                    service.AddScoped(validatorType, serviceType);
                    break;
                case ServiceLifetime.Transient:
                    service.AddTransient(validatorType, serviceType);
                    break;
            }
        });
        return service;
    }
    public static void AddValidatorControl(this IServiceCollection service)
    {
        service.AddSingleton(pro =>
        {
            return new ValidatorControl(service.BuildServiceProvider()) as IValidatorControl;
        });
    }
}
