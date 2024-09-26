using FluentValidation.Results;

namespace Validation;

public class ValidatorResult
{
    public int ErrorCount { get; private set; } = 0;
    public bool IsValid { get; private set; } = true;
    public List<ErrorMessage>? ErrorMessage { get; private set; } = [];
    public ValidatorResult()
    {

    }
    public ValidatorResult SetErrorMessage(List<ValidationFailure> failures)
    {
        ErrorCount = failures.Count;
        IsValid = ErrorCount == 0;
        if (!IsValid)
            failures.ForEach(failure =>
            {
                ErrorMessage.Add(new ErrorMessage(failure.PropertyName, failure.ErrorMessage));
            });

        return this;
    }
}
public record ErrorMessage(string? PropertyName, string? Message);
