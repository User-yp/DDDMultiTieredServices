using CacheServices.RedisService;
using IdentityService.Domain.Entity;
using IdentityService.Domain.Result;
using Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace IdentityService.Domain;

public class IdDomainService
{
    private readonly IIdRepository repository;
    private readonly ITokenService tokenService;
    private readonly IOptions<JWTOptions> optJWT;
    private readonly ISmsSender smsSender;
    private readonly IRedisService redisService;

    public IdDomainService(IIdRepository repository, ITokenService tokenService, IOptions<JWTOptions> optJWT, ISmsSender smsSender,IRedisService redisService)
    {
        this.repository = repository;
        this.tokenService = tokenService;
        this.optJWT = optJWT;
        this.smsSender = smsSender;
        this.redisService = redisService;
    }

    private async Task<SignInResult> CheckUserNameAndPwdAsync(string userName, string password)
    {
        var user = await repository.FindByNameAsync(userName);
        if (user == null)
        {
            return SignInResult.Failed;
        }
        //CheckPasswordSignInAsync会对于多次重复失败进行账号禁用
        var result = await repository.CheckForSignInAsync(user, password, true);
        return result;
    }
    private async Task<SignInResult> CheckPhoneNumAndPwdAsync(string phoneNum, string password)
    {
        var user = await repository.FindByPhoneNumberAsync(phoneNum);
        if (user == null)
        {
            return SignInResult.Failed;
        }
        var result = await repository.CheckForSignInAsync(user, password, true);
        return result;
    }
    private async Task<(SignInResult, string? data)> CheckPhoneNumAsync(string phoneNum, string smsCode)
    {
        var user = await repository.FindByPhoneNumberAsync(phoneNum);
        if (user == null)
            return (SignInResult.Failed, null);
        (var resbool, VerifySmsCodeResult? res) = await repository.VerifySmsCodeAsync(phoneNum, smsCode);
        if (!resbool)
            return (SignInResult.Failed, res.Data);
        var result = await repository.CheckForPhoneAsync(phoneNum, true);
        return (result, res.Data);
    }
    private async Task<bool> CheckJwtVersionAsync(User user, string token)
    {
        if (!await redisService.HashFieldsExistsAsync(nameof(UserJwtVersion), [user.Id.ToString()]))
        {
            await redisService.HashSetorCreateFieldsAsync(nameof(UserJwtVersion), new ConcurrentDictionary<string, string>
            {
                [user.Id.ToString()] = token
            });
            return true;
        }
        var dict = await redisService.HashGetFieldsAsync(nameof(UserJwtVersion), [user.Id.ToString()]);
        if (dict[user.Id.ToString()] != token)
        {
            await redisService.HashSetorCreateFieldsAsync(nameof(UserJwtVersion), new ConcurrentDictionary<string, string>
            {
                [user.Id.ToString()] = token
            });
            return false;
        }
        return true;
    }
    public async Task<(SignInResult Result, string? Token, bool token)> LoginByPhoneAndPwdAsync(string phoneNum, string password)
    {
        var checkResult = await CheckPhoneNumAndPwdAsync(phoneNum, password);
        if (checkResult.Succeeded)
        {
            var user = await repository.FindByPhoneNumberAsync(phoneNum);
            string token = await BuildTokenAsync(user);
            bool jwtCheckRes = await CheckJwtVersionAsync(user, token);
            return (SignInResult.Success, token, jwtCheckRes);
        }
        else
        {
            return (checkResult, null, true);
        }
    }
    public async Task<(SignInResult Result, string? Token, bool token)> LoginByUserNameAndPwdAsync(string userName, string password)
    {
        var checkResult = await CheckUserNameAndPwdAsync(userName, password);
        if (checkResult.Succeeded)
        {
            var user = await repository.FindByNameAsync(userName);
            string token = await BuildTokenAsync(user);
            bool jwtCheckRes = await CheckJwtVersionAsync(user, token);
            return (SignInResult.Success, token, jwtCheckRes);
        }
        else
        {
            return (checkResult, null, true);
        }
    }
    public async Task<(SignInResult Result, string? Token, bool token)> LoginByPhoneAndSmsCodeAsync(string phoneNum, string smsCode)
    {
        (var signInResult, var verifyRes) = await CheckPhoneNumAsync(phoneNum, smsCode);
        if (signInResult.Succeeded)
        {
            var user = await repository.FindByPhoneNumberAsync(phoneNum);
            string token = await BuildTokenAsync(user);
            bool jwtCheckRes = await CheckJwtVersionAsync(user, token);
            return (SignInResult.Success, token, jwtCheckRes);
        }
        else
            return (signInResult, verifyRes, true);
    }
    private async Task<string> BuildTokenAsync(User user)
    {
        var roles = await repository.GetRolesAsync(user);
        List<Claim> claims = [new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())];
        foreach (string role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        return tokenService.BuildToken(claims, optJWT.Value);
    }
    public async Task SendCodeAsync(string phoneNum, string code)
    {
        await smsSender.SendAsync(phoneNum, code);
        await repository.SavePhoneNumberCodeAsync(phoneNum, code, 300);
    }
}
