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
        var ser = InitValidatorService(service, assemblies);
        InitValidatorControl(service.BuildServiceProvider(), assemblies);
        return ser;
    }
    public static IServiceCollection AddFluentValidation(this IServiceCollection service, params Assembly[] assemblies)
    {
        var ser = service.InitValidatorService(assemblies);
        InitValidatorControl(service.BuildServiceProvider(), assemblies);
        return ser;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="service"></param>
    /// <param name="assemblies">所有程序集</param>
    /// <returns></returns>
    private static IServiceCollection InitValidatorService(IServiceCollection service, IEnumerable<Assembly> assemblies)
    {
        //获取有ServiceAttribute特性的所有类
        List<Type> typeAttribute = assemblies
            .SelectMany(x => x.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract
                    && t.GetCustomAttributes(typeof(ValidatorAttribute), false).Length > 0)
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="service"></param>
    /// <param name="assemblies">指定程序集</param>
    /// <returns></returns>
    public static IServiceCollection InitValidatorService(this IServiceCollection service, params Assembly[] assemblies)
    {
        //获取有ServiceAttribute特性的所有类
        List<Type> typeAttribute = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract
                && t.GetCustomAttributes(typeof(ValidatorAttribute), false).Length != 0)
                .ToList();

        /*List<Type> typeAttribute = [];
        if (assemblies.Length == 1)
        {
            typeAttribute = assemblies[0].GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract
                && t.GetCustomAttributes(typeof(ValidatorAttribute), false).Length != 0)
                .ToList();
        }
        else
        {
            foreach (Assembly assembly in assemblies)
            {
                List<Type> types = assembly.GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract
                    && t.GetCustomAttributes(typeof(ValidatorAttribute), false).Length != 0)
                    .ToList();
                typeAttribute.AddRange(types);
            }
        }*/


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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="assemblies">所有程序集</param>
    /// <exception cref="ArgumentException">No Method Named Init</exception>
    public static void InitValidatorControl(ServiceProvider serviceProvider, IEnumerable<Assembly> assemblies)
    {
        //获取有ServiceAttribute特性的所有类
        var typeAttribute = assemblies
            .SelectMany(x => x.GetTypes())
            .Where(t => t.IsClass && t.GetCustomAttributes(typeof(ValidatorControlAttribute), false).Length > 0)
            .ToList();

        if (typeAttribute.Count > 0)
        {
            typeAttribute.ForEach(impl =>
            {
                var methodInfo = impl.GetMethod("Init", BindingFlags.Public | BindingFlags.Static)
                    ?? throw new ArgumentException("No Method Named Init");

                methodInfo.Invoke(null, [serviceProvider]);
            });
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="assemblies">指定程序集</param>
    /// <exception cref="ArgumentException">No Method Named Init</exception>
    public static void InitValidatorControl(ServiceProvider serviceProvider, params Assembly[] assemblies)
    {
        //获取有ServiceAttribute特性的所有类
        var typeAttribute = assemblies
            .SelectMany(x => x.GetTypes())
            .Where(t => t.IsClass && t.GetCustomAttributes(typeof(ValidatorControlAttribute), false).Length > 0)
            .ToList();

        if (typeAttribute.Count > 0)
        {
            typeAttribute.ForEach(impl =>
            {
                var methodInfo = impl.GetMethod("Init", BindingFlags.Public | BindingFlags.Static)
                    ?? throw new ArgumentException("No Method Named Init");

                methodInfo.Invoke(null, [serviceProvider]);
            });
        }
    }
}
