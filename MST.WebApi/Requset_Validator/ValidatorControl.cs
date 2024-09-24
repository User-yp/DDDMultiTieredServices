using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MTS.Infrastructure.Migrations;
using Validation;

namespace MTS.WebApi.Requset_Validator;

public static class ValidatorControl
{
    public static IValidator<TestRequset> TestRequset;
    public static IValidator<AddOrderRequset> AddOrderRequset;

    public static void init(ServiceProvider service)
    {
        TestRequset = service.GetService<IValidator<TestRequset>>();
        AddOrderRequset = service.GetService<IValidator<AddOrderRequset>>();
    }
}
