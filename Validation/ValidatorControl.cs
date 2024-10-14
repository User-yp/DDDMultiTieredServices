using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validation;

public class ValidatorControl : IValidatorControl
{
    public readonly ServiceProvider _service;
    public ValidatorControl(ServiceProvider service)
    {
        _service = service;
    }

    public IValidator<T> GetValidator<T>(T tType)
    {
        var validator = _service.GetService<IValidator<T>>();
        if (validator == null)
            throw new ApplicationException(" 'validator' Not Registered ");
        return validator;
    }

    public async Task<ValidatorResult> RequestValidateAsync<T>(T request) where T : IValidatorBase
    {
        var val = await GetValidator(request).ValidateAsync(new ValidationContext<T>(request));
        var result = new ValidatorResult();
        if (!val.IsValid)
            return result.SetErrorMessage(val.Errors);
        return result;
    }
}
