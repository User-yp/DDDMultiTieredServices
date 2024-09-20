using CacheServices.RedisService;
using IdentityService.Domain;
using IdentityService.Domain.Entity;
using IdentityService.Domain.Result;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text;

namespace IdentityService.Infrastructure;

class IdRepository : IIdRepository
{
    private readonly IdUserManager userManager;
    private readonly RoleManager<Role> roleManager;
    private readonly ILogger<IdRepository> logger;
    private readonly IRedisService redisService;

    public IdRepository(IdUserManager userManager, RoleManager<Role> roleManager, ILogger<IdRepository> logger,IRedisService redisService)
    {
        this.userManager = userManager;
        this.roleManager = roleManager;
        this.logger = logger;
        this.redisService = redisService;
    }

    public Task<User?> FindByPhoneNumberAsync(string phoneNum)
    {
        return userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNum);
    }

    public Task<User?> FindByIdAsync(Guid userId)
    {
        return userManager.FindByIdAsync(userId.ToString());
    }

    public Task<User?> FindByNameAsync(string userName)
    {
        return userManager.FindByNameAsync(userName);
    }

    public List<User> FindBySearchNameAsync(string userName)
    {
        var users = userManager.Users.Where(u => u.IsDeleted == false && u.UserName.ToLower().Contains(userName.ToLower())
        || u.UserName.StartsWith(userName)
        || u.UserName.EndsWith(userName)
        || EF.Functions.Like(u.UserName, $"%{userName}%")).ToList();
        if (users.Count != 0)
            return users;
        return null;
    }

    public List<User> FindAllLockedUsers()
    {
        var users = userManager.Users.Where(u => u.LockoutEnd != null && u.LockoutEnd > DateTimeOffset.UtcNow)
            .ToList();
        if (users.Count != 0)
            return users;
        return null;
    }

    public Task<IdentityResult> CreateAsync(User user, string password)
    {
        return userManager.CreateAsync(user, password);
    }

    public Task<IdentityResult> AccessFailedAsync(User user)
    {
        return userManager.AccessFailedAsync(user);
    }

    public async Task<IdentityResult> ChangePhoneNumberAsync(Guid userId, string phoneNum, string token)
    {
        var user = await userManager.FindByIdAsync(userId.ToString()) ?? throw new ArgumentException($"{userId}的用户不存在");
        var changeResult = await this.userManager.ChangePhoneNumberAsync(user, phoneNum, token);
        if (!changeResult.Succeeded)
        {
            await userManager.AccessFailedAsync(user);
            string errMsg = changeResult.Errors.SumErrors();
            logger.LogWarning($"{phoneNum}ChangePhoneNumberAsync失败，错误信息{errMsg}");
            return IdentityResult.Failed(new IdentityError { Code = "ChangePhoneNumberFailed", Description = "errMsg" });
        }
        else
        {
            await ConfirmPhoneNumberAsync(user.Id);//确认手机号
            return IdentityResult.Success;
        }
    }

    public async Task<IdentityResult> ChangeUserNameAsync(Guid userId, string userName)
    {
        var user = await userManager.FindByIdAsync(userId.ToString()) ?? throw new ArgumentException($"{userId}的用户不存在");
        if (user.UserName == userName)
            return IdentityResult.Failed(new IdentityError { Code = "DuplicateUserName", Description = $"用户名 '{userName}' 不能与旧名称相同" });

        var existingUser = await userManager.FindByNameAsync(userName);
        //是否启用重名检查
        if (existingUser != null && existingUser.Id != user.Id)
            return IdentityResult.Failed(new IdentityError { Code = "DuplicateUserName", Description = $"用户名 '{userName}' 已被使用" });
        user.UserName = userName;
        var updateResult = await userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            await userManager.AccessFailedAsync(user);
            string errMsg = string.Join(", ", updateResult.Errors.Select(e => e.Description));
            logger.LogWarning($"用户名 {userName} 修改失败，错误信息: {errMsg}");
            return IdentityResult.Failed(updateResult.Errors.ToArray());
        }
        else
            return IdentityResult.Success;
    }

    public async Task<IdentityResult> ChangePasswordAsync(Guid userId, string password)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var resetPwdResult = await userManager.ResetPasswordAsync(user, token, password);
        return resetPwdResult;
    }
    public async Task<IdentityResult> ChangePasswordAsync(Guid userId, string password, string newPassword)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (!await userManager.CheckPasswordAsync(user, password))
            return IdentityResult.Failed(new IdentityError { Code = "Invalid Password", Description = "原密码不正确" });

        if (password == newPassword)
            return IdentityResult.Failed(new IdentityError { Code = "Invalid Password", Description = "新密码不能与原密码相同" });
        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var resetPwdResult = await userManager.ResetPasswordAsync(user, token, newPassword);
        return resetPwdResult;
    }

    public async Task<IdentityResult> RetrievePasswordAsync(Guid userId, string password, string smsCode)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        (var resBool, var res) = await VerifySmsCodeAsync(user.PhoneNumber, smsCode);
        if (resBool)
        {
            if (await userManager.CheckPasswordAsync(user, password))
                return IdentityResult.Failed(new IdentityError { Code = "Invalid Password", Description = "新密码不能与原密码相同" });
            else
            {
                await ChangePasswordAsync(userId, password);
                await redisService.KeyDeleteAsync(user.PhoneNumber);
                return IdentityResult.Success;
            }
        }
        else
            return IdentityResult.Failed(new IdentityError { Code = "VerifySmsCodeError", Description = $"{res.Data}" });
    }

    public Task<string> GenerateChangePhoneNumberTokenAsync(User user, string phoneNumber)
    {
        return this.userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);
    }

    public Task<IList<string>> GetRolesAsync(User user)
    {
        return userManager.GetRolesAsync(user);
    }

    public async Task<IdentityResult> AddToRoleAsync(User user, string roleName)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            Role role = new() { Name = roleName };
            var result = await roleManager.CreateAsync(role);
            if (result.Succeeded == false)
            {
                return result;
            }
        }
        return await userManager.AddToRoleAsync(user, roleName);
    }
    /// <summary>
    /// 尝试登录，如果lockoutOnFailure为true，则登录失败还会自动进行lockout计数
    /// </summary>
    /// <param name="user"></param>
    /// <param name="password"></param>
    /// <param name="lockoutOnFailure"></param>
    /// <returns></returns>
    /// <exception cref="ApplicationException"></exception>
    public async Task<SignInResult> CheckForSignInAsync(User user, string password, bool lockoutOnFailure)
    {
        if (await userManager.IsLockedOutAsync(user))
        {
            return SignInResult.LockedOut;
        }
        var success = await userManager.CheckPasswordAsync(user, password);
        if (success)
        {
            return SignInResult.Success;
        }
        else
        {
            if (lockoutOnFailure)
            {
                var r = await AccessFailedAsync(user);
                if (!r.Succeeded)
                {
                    throw new ApplicationException("AccessFailed failed");
                }
            }
            return SignInResult.Failed;
        }
    }

    public async Task<SignInResult> CheckForPhoneAsync(string phoneNumber, bool lockoutOnFailure)
    {
        var user = await FindByPhoneNumberAsync(phoneNumber);
        if (await userManager.IsLockedOutAsync(user))
        {
            return SignInResult.LockedOut;
        }
        if (user != null)
        {
            return SignInResult.Success;
        }
        else
        {
            if (lockoutOnFailure)
            {
                var r = await AccessFailedAsync(user);
                if (!r.Succeeded)
                {
                    throw new ApplicationException("AccessFailed failed");
                }
            }
            return SignInResult.Failed;
        }
    }

    public async Task ConfirmPhoneNumberAsync(Guid id)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == id) ?? throw new ArgumentException($"用户找不到，id={id}", nameof(id));
        user.PhoneNumberConfirmed = true;
        await userManager.UpdateAsync(user);
    }

    public async Task UpdatePhoneNumberAsync(Guid id, string phoneNum)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == id) ?? throw new ArgumentException($"用户找不到，id={id}", nameof(id));
        user.PhoneNumber = phoneNum;
        await userManager.UpdateAsync(user);
    }

    public async Task<List<IdentityResult>> ModifyUserInforByIdAsync(Guid id, string? UserName, string? PhoneNumber, string? Password)
    {
        List<IdentityResult> results = [];
        var user = await FindByIdAsync(id);
        if (user == null)
        {
            results.Add(IdentityResult.Failed(new IdentityError() { Code = "NoUser", Description = $"用户名 '{id}'不存在" }));
            return results;
        }
        if (!string.IsNullOrEmpty(UserName))
            results.Add(await ChangeUserNameAsync(id, UserName));

        if (!string.IsNullOrEmpty(PhoneNumber))
            results.Add(await ChangePhoneNumberAsync(id, PhoneNumber, await GenerateChangePhoneNumberTokenAsync(user, PhoneNumber)));

        if (!string.IsNullOrEmpty(Password))
            results.Add(await ChangePasswordAsync(id, Password));
        return results;
    }

    public async Task<List<IdentityResult>> UserModifyInforByIdAsync(Guid id, string? UserName, string? PhoneNumber, string? Password, string? NewPassword)
    {
        List<IdentityResult> results = [];
        var user = await FindByIdAsync(id);
        if (user == null)
        {
            results.Add(IdentityResult.Failed(new IdentityError() { Code = "NoUser", Description = $"用户名 '{id}'不存在" }));
            return results;
        }
        if (! string.IsNullOrEmpty(UserName))
            results.Add(await ChangeUserNameAsync(id, UserName));

        if (!string.IsNullOrEmpty(PhoneNumber))
            results.Add(await ChangePhoneNumberAsync(id, PhoneNumber, await GenerateChangePhoneNumberTokenAsync(user, PhoneNumber)));

        if (!string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(NewPassword))
            results.Add(await ChangePasswordAsync(id, Password, NewPassword));
        return results;
    }
    /// <summary>
    /// 软删除
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IdentityResult> RemoveUserAsync(Guid id)
    {
        var user = await FindByIdAsync(id);
        var userLoginStore = userManager.UserLoginStore;
        var noneCT = default(CancellationToken);
        //一定要删除aspnetuserlogins表中的数据，否则再次用这个外部登录登录的话
        //就会报错：The instance of entity type 'IdentityUserLogin<Guid>' cannot be tracked because another instance with the same key value for {'LoginProvider', 'ProviderKey'} is already being tracked.
        //而且要先删除aspnetuserlogins数据，再软删除User
        var logins = await userLoginStore.GetLoginsAsync(user, noneCT);
        foreach (var login in logins)
        {
            await userLoginStore.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey, noneCT);
        }
        user.SoftDelete();
        var result = await userManager.UpdateAsync(user);
        return result;
    }

    public async Task<IdentityResult> RemoveUserByNameAsync(string userName)
    {
        var user = await FindByNameAsync(userName);
        var userLoginStore = userManager.UserLoginStore;
        var noneCT = default(CancellationToken);
        //一定要删除aspnetuserlogins表中的数据，否则再次用这个外部登录登录的话
        //就会报错：The instance of entity type 'IdentityUserLogin<Guid>' cannot be tracked because another instance with the same key value for {'LoginProvider', 'ProviderKey'} is already being tracked.
        //而且要先删除aspnetuserlogins数据，再软删除User
        var logins = await userLoginStore.GetLoginsAsync(user, noneCT);
        foreach (var login in logins)
        {
            await userLoginStore.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey, noneCT);
        }
        user.SoftDelete();
        var result = await userManager.UpdateAsync(user);
        return result;
    }

    public async Task<(IdentityResult?, bool)> ResetLockedUserByName(string userName)
    {
        User? user = await userManager.FindByNameAsync(userName);
        if (user == null)
            return (null, false);
        if (user.LockoutEnd == null || user.LockoutEnd <= DateTimeOffset.UtcNow)
            return (null, false);
        var res = await userManager.SetLockoutEndDateAsync(user, null);
        if (res.Succeeded)
            return (res, true);
        return (null, false);

    }

    private static IdentityResult ErrorResult(string msg)
    {
        IdentityError idError = new IdentityError { Description = msg };
        return IdentityResult.Failed(idError);
    }

    public async Task<(IdentityResult, User?, string? password)> AddAdminUserAsync(string userName, string phoneNum)
    {
        if (await FindByNameAsync(userName) != null)
        {
            return (ErrorResult($"已经存在用户名{userName}"), null, null);
        }
        if (await FindByPhoneNumberAsync(phoneNum) != null)
        {
            return (ErrorResult($"已经存在手机号{phoneNum}"), null, null);
        }
        User user = new(userName)
        {
            PhoneNumber = phoneNum,
            PhoneNumberConfirmed = true
        };
        string password = GeneratePassword();
        var result = await CreateAsync(user, password);
        if (!result.Succeeded)
        {
            return (result, null, null);
        }
        result = await AddToRoleAsync(user, "Admin");
        if (!result.Succeeded)
        {
            return (result, null, null);
        }
        return (IdentityResult.Success, user, password);
    }

    public async Task<(IdentityResult, User?)> AddUserAsync(string userName, string phoneNum, string password)
    {
        if (await FindByPhoneNumberAsync(userName) != null)
        {
            return (ErrorResult($"已经存在用户名{userName}"), null);
        }
        if (await FindByPhoneNumberAsync(phoneNum) != null)
        {
            return (ErrorResult($"已经存在手机号{phoneNum}"), null);
        }
        User user = new(userName)
        {
            PhoneNumber = phoneNum,
            PhoneNumberConfirmed = true
        };
        IdentityResult? result = await CreateAsync(user, password);
        if (!result.Succeeded)
        {
            return (result, null);
        }
        result = await AddToRoleAsync(user, "User");
        if (!result.Succeeded)
        {
            return (result, null);
        }
        return (IdentityResult.Success, user);
    }

    public async Task<(IdentityResult, User?, string? password)> ResetPasswordAsync(Guid id)
    {
        var user = await FindByIdAsync(id);
        if (user == null)
        {
            return (ErrorResult("用户没找到"), null, null);
        }
        string password = GeneratePassword();
        string token = await userManager.GeneratePasswordResetTokenAsync(user);
        var result = await userManager.ResetPasswordAsync(user, token, password);
        if (!result.Succeeded)
        {
            return (result, null, null);
        }
        return (IdentityResult.Success, user, password);
    }

    private string GeneratePassword()
    {
        var options = userManager.Options.Password;
        int length = options.RequiredLength;
        bool nonAlphanumeric = options.RequireNonAlphanumeric;
        bool digit = options.RequireDigit;
        bool lowercase = options.RequireLowercase;
        bool uppercase = options.RequireUppercase;
        StringBuilder password = new StringBuilder();
        Random random = new Random();
        while (password.Length < length)
        {
            char c = (char)random.Next(32, 126);
            password.Append(c);
            if (char.IsDigit(c))
                digit = false;
            else if (char.IsLower(c))
                lowercase = false;
            else if (char.IsUpper(c))
                uppercase = false;
            else if (!char.IsLetterOrDigit(c))
                nonAlphanumeric = false;
        }

        if (nonAlphanumeric)
            password.Append((char)random.Next(33, 48));
        if (digit)
            password.Append((char)random.Next(48, 58));
        if (lowercase)
            password.Append((char)random.Next(97, 123));
        if (uppercase)
            password.Append((char)random.Next(65, 91));
        return password.ToString();
    }

    public async Task SavePhoneNumberCodeAsync(string phoneNum, string code, double timeSpan)
    {
        await redisService.StringSetAsync(phoneNum, code, TimeSpan.FromSeconds(timeSpan));
    }

    public async Task<(bool, VerifySmsCodeResult)> VerifySmsCodeAsync(string phoneNum, string code)
    {
        if (!await redisService.KeyExistsAsync(phoneNum))
            return (false, new VerifySmsCodeResult("验证码已过期"));
        var smsCode = await redisService.StringGetAsync<string>(phoneNum);
        if (smsCode != code)
            return (false, new VerifySmsCodeResult("验证码错误"));
        return (true, new VerifySmsCodeResult("验证码正确"));
    }
}
