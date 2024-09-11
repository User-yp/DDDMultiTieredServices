using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Validation;

public static class ValidatorExtensions
{
    /// <summary>
    /// 按特性中的生命周期注入业务组件
    /// </summary>
    /// <param name="service"></param>
    public static void AddFluentValidation(this IServiceCollection service, IEnumerable<Assembly> assemblies)
    {
        //获取有ServiceAttribute特性的所有类
        List<Type> types = assemblies
            .SelectMany(x => x.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract
                    && t.GetCustomAttributes(typeof(ValidatorAttribute), false).Length > 0)
            .ToList();

        types.ForEach(impl =>
        {
            //获取生命周期
            var lifetime = impl.GetCustomAttribute<ValidatorAttribute>().LifeTime;
            //获取ServiceType
            var serviceType = impl.GetCustomAttribute<ValidatorAttribute>().ServiceType;
            //写入泛型参数，获取IValidator<>类型
            var validatorType = typeof(IValidator<>).MakeGenericType(impl);

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
    }
}
