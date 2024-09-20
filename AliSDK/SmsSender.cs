using AlibabaCloud.SDK.Dysmsapi20170525.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tea;

namespace AliSDK;

public static class SmsSender
{
    public static AlibabaCloud.SDK.Dysmsapi20170525.Client CreateClient(string accessKeyId, string accessKeySecret)
    {
        AlibabaCloud.OpenApiClient.Models.Config config = new()
        {
            AccessKeyId = accessKeyId,
            AccessKeySecret = accessKeySecret
        };

        config.Endpoint = "dysmsapi.aliyuncs.com";
        return new AlibabaCloud.SDK.Dysmsapi20170525.Client(config);
    }
    public static void UseSms(string phone, string token)
    {
        string? accessKeyId = Environment.GetEnvironmentVariable("accessKeyId");
        string? accessKeySecret = Environment.GetEnvironmentVariable("accessKeySecret");
        var client = CreateClient(accessKeyId, accessKeySecret);

        SendSmsRequest sendSmsRequest = new SendSmsRequest()
        {
            PhoneNumbers = phone,
            SignName = "亚普网",
            TemplateCode = "SMS_465413181",
            TemplateParam = $"{{\"code\":\"{token}\"}}"
        };

        try
        {
            client.SendSmsWithOptionsAsync(sendSmsRequest, new AlibabaCloud.TeaUtil.Models.RuntimeOptions());
        }
        catch (TeaException error)
        {

            AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
        }
        catch (Exception e)
        {
            TeaException error = new TeaException(new Dictionary<string, object>
                {
                    { "message",e.Message}
                });

            AlibabaCloud.TeaUtil.Common.AssertAsString(e.Message);
        }
    }
}

