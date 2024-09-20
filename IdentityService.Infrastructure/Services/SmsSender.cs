using IdentityService.Domain;
using Microsoft.Extensions.Logging;

namespace IdentityService.Infrastructure.Services;

public class SmsSender : ISmsSender
{
    private readonly ILogger<SmsSender> logger;
    public SmsSender(ILogger<SmsSender> logger)
    {
        this.logger = logger;
    }
    public Task SendAsync(string phoneNum, string code)
    {
        AliSDK.SmsSender.UseSms(phoneNum, code);
        logger.LogInformation($"向手机号{phoneNum}发送验证码：{code}");
        return Task.CompletedTask;
    }
}
