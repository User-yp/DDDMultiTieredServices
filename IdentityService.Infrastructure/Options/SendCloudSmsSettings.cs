using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Infrastructure.Options;

public class SendCloudSmsSettings
{
    public string SmsUser { get; set; }
    public string SmsKey { get; set; }
}
