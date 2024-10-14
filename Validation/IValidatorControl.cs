using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validation;

public interface IValidatorControl
{
    IValidator<T> GetValidator<T>(T tType);
    Task<ValidatorResult> RequestValidateAsync<T>(T request) where T : IValidatorBase;
}
