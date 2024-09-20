using IdentityService.Domain.Entity;
using IdentityService.Domain.Result;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain;

public interface IIdRepository
{
    Task<User?> FindByIdAsync(Guid userId);//根据Id获取用户
    Task<User?> FindByNameAsync(string userName);//根据用户名获取用户
    List<User> FindBySearchNameAsync(string userName);//根据用户名模糊查询
    Task<User?> FindByPhoneNumberAsync(string phoneNum);//根据手机号获取用户
    List<User> FindAllLockedUsers();//获取所有锁定用户
    Task<IdentityResult> CreateAsync(User user, string password);//创建用户
    Task<IdentityResult> AccessFailedAsync(User user);//记录一次登陆失败

    /// <summary>
    /// 生成重置密码的令牌
    /// </summary>
    /// <param name="user"></param>
    /// <param name="phoneNumber"></param>
    /// <returns></returns>
    Task<string> GenerateChangePhoneNumberTokenAsync(User user, string phoneNumber);
    /// <summary>
    /// 检查VCode，然后设置用户手机号为phoneNum
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="phoneNum"></param>
    /// <param name="code"></param>
    /// <returns></returns>

    Task<IdentityResult> ChangePhoneNumberAsync(Guid userId, string phoneNum, string token);

    Task<IdentityResult> ChangeUserNameAsync(Guid userId, string userName);

    Task<IdentityResult> ChangePasswordAsync(Guid userId, string password);

    /// <summary>
    /// 修改密码
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    Task<IdentityResult> ChangePasswordAsync(Guid userId, string password, string newPassword);

    Task<IdentityResult> RetrievePasswordAsync(Guid userId, string password, string smsCode);

    /// <summary>
    /// 获取用户的角色
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    Task<IList<string>> GetRolesAsync(User user);

    /// <summary>
    /// 把用户user加入角色role
    /// </summary>
    /// <param name="user"></param>
    /// <param name="role"></param>
    /// <returns></returns>
    Task<IdentityResult> AddToRoleAsync(User user, string role);
    /// <summary>
    /// 为了登录而检查用户名、密码是否正确
    /// </summary>
    /// <param name="user"></param>
    /// <param name="password"></param>
    /// <param name="lockoutOnFailure">如果登录失败，则记录一次登陆失败</param>
    /// <returns></returns>
    public Task<SignInResult> CheckForSignInAsync(User user, string password, bool lockoutOnFailure);
    /// <summary>
    /// 为了登录而检查手机号是否正确
    /// </summary>
    /// <param name="user"></param>
    /// <param name="lockoutOnFailure">如果登录失败，则记录一次登陆失败</param>
    /// <returns></returns>
    public Task<SignInResult> CheckForPhoneAsync(string phoneNum, bool lockoutOnFailure);
    /// <summary>
    /// 确认手机号
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task ConfirmPhoneNumberAsync(Guid id);

    /// <summary>
    /// 修改手机号
    /// </summary>
    /// <param name="id"></param>
    /// <param name="phoneNum"></param>
    /// <returns></returns>
    public Task UpdatePhoneNumberAsync(Guid id, string phoneNum);

    public Task<List<IdentityResult>> ModifyUserInforByIdAsync(Guid id, string? userName, string? phoneNumber, string? password);

    public Task<List<IdentityResult>> UserModifyInforByIdAsync(Guid id, string? userName, string? phoneNumber, string? password, string? newPassword);

    /// <summary>
    /// 根据Guid删除用户
    /// </summary>
    /// <param name="id">Guid</param>
    /// <returns></returns>
    public Task<IdentityResult> RemoveUserAsync(Guid id);
    /// <summary>
    /// 根据用户名删除用户
    /// </summary>
    /// <param name="userName">用户名</param>
    /// <returns></returns>
    public Task<IdentityResult> RemoveUserByNameAsync(string userName);
    Task<(IdentityResult?, bool)> ResetLockedUserByName(string userName);

    /// <summary>
    /// 添加管理员
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="phoneNum"></param>
    /// <returns>返回值第三个是生成的密码</returns>
    public Task<(IdentityResult, User?, string? password)> AddAdminUserAsync(string userName, string phoneNum);
    /// <summary>
    /// 添加用户
    /// </summary>
    /// <param name="userName">用户名</param>
    /// <param name="phoneNum">手机号</param>
    /// <param name="password">密码</param>
    /// <returns></returns>
    public Task<(IdentityResult, User?)> AddUserAsync(string userName, string phoneNum, string password);
    /// <summary>
    /// 重置密码。
    /// </summary>
    /// <param name="id"></param>
    /// <returns>返回值第三个是生成的密码</returns>
    public Task<(IdentityResult, User?, string? password)> ResetPasswordAsync(Guid id);
    /// <summary>
    /// 保存用户验证码
    /// </summary>
    /// <param name="phone"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    public Task SavePhoneNumberCodeAsync(string phoneNum, string code, double timeSpan);
    public Task<(bool, VerifySmsCodeResult)> VerifySmsCodeAsync(string phoneNum, string code);
}
