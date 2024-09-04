using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain;

public class OperateError
{
    /// <summary>
    /// Gets or sets the code for this error.
    /// </summary>
    /// <value>
    /// The code for this error.
    /// </value>
    public string Code { get; set; } = default!;

    /// <summary>
    /// Gets or sets the description for this error.
    /// </summary>
    /// <value>
    /// The description for this error.
    /// </value>
    public string Description { get; set; } = default!;
}
