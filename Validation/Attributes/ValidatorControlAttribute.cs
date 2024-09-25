namespace Validation.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ValidatorControlAttribute : Attribute
{
    private Type? ValidatorType { get; set; }

    public ValidatorControlAttribute()
    {

    }
    public ValidatorControlAttribute(Type type)
    {
        if (!CheckValidatorType(type))
        {
            throw new ArgumentException("ValidatorControl Must be static");
        }
        ValidatorType = type;
    }

    private static bool CheckValidatorType(Type type) => type.IsAbstract && type.IsSealed;
}