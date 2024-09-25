using FluentValidation;

namespace Validation.Extensions;

public static class RequestExtensions
{
    public static async Task<ValidatorResult> RequestValidateAsync<T>(this IValidator validator, T request) where T : RequsetBase
    {
        var val = await validator.ValidateAsync(new ValidationContext<T>(request));
        if (!val.IsValid)
        {
            List<string> errors = [];
            foreach (var error in val.Errors)
            {
                errors.Add(error.ErrorMessage);
            }
            return new ValidatorResult(401, errors);
        }
        return new ValidatorResult(200, null);
    }
}
