using IdentityService.Domain.Entity;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

namespace IdentityService.Domain;

public class IdUserValidator : UserValidator<User>
{
    public override async Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user)
    {
        var result = await base.ValidateAsync(manager, user);

        if (!Regex.IsMatch(user.UserName, @"^[\u4e00-\u9fa5a-zA-Z0-9]*$"))
        {
            result = IdentityResult.Failed(new IdentityError { Code = "InvalidUserName", Description = "Username can only contain Chinese characters, letters, or digits." });
        }
        return result;
    }
}
