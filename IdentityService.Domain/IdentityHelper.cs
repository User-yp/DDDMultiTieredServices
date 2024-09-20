using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain;

public static class IdentityHelper
{
    public static string SumErrors(this IEnumerable<IdentityError> errors)
    {
        var strs = errors.Select(e => $"code={e.Code},message={e.Description}");
        return string.Join('\n', strs);
    }
}
