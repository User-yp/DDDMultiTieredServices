using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MTS.Infrastructure.Migrations;
using Validation;
using Validation.Attributes;

namespace MTS.WebApi.Requset_Validator;
[ValidatorControl]
public static class ValidatorControl
{
    public static IValidator<TestRequset> TestRequset;
    public static IValidator<AddOrderRequset> AddOrderRequset;

    public static void Init(ServiceProvider service)
    {
        TestRequset = service.GetService<IValidator<TestRequset>>();
        AddOrderRequset = service.GetService<IValidator<AddOrderRequset>>();
    }
}
