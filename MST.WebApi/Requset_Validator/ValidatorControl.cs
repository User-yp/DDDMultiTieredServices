using FluentValidation;
using Validation;

namespace MTS.WebApi.Requset_Validator;

public class ValidatorControl: IValidatorControl
{
    public IValidator<TestRequset> TestRequset;
    public IValidator<AddOrderRequset> AddOrderRequset;
}
