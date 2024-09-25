using FluentValidation;
using Validation;
using Validation.Attributes;

namespace MTS.WebApi.Requset_Validator;

[Validator(typeof(TestRequsetValidator), ServiceLifetime.Scoped)]
public record TestRequset(int num,string length):RequsetBase;
public class TestRequsetValidator : AbstractValidator<TestRequset>
{
    public TestRequsetValidator()
    {
        RuleFor(e => e.num).NotNull().NotEmpty().LessThanOrEqualTo(100);
        RuleFor(e => e.length).NotEmpty().NotEmpty().MaximumLength(20).MinimumLength(2);
    }
}