using System.Globalization;

namespace Domain;

public class OperateResult
{
    private static readonly OperateResult _success = new() { Succeeded = true };
    private readonly List<OperateError> _errors = [];
    public bool Succeeded { get; protected set; }
    public IEnumerable<OperateError> Errors => _errors;
    public static OperateResult Success => _success;
    public static OperateResult Failed(params OperateError[] errors)
    {
        var result = new OperateResult { Succeeded = false };
        if (errors != null)
        {
            result._errors.AddRange(errors);
        }
        return result;
    }
    internal static OperateResult Failed(List<OperateError>? errors)
    {
        var result = new OperateResult { Succeeded = false };
        if (errors != null)
        {
            result._errors.AddRange(errors);
        }
        return result;
    }
    public override string ToString()
    {
        return Succeeded ?
               "Succeeded" :
               string.Format(CultureInfo.InvariantCulture, "{0} : {1}", "Failed", string.Join(",", Errors.Select(x => x.Code).ToList()));
    }
}
