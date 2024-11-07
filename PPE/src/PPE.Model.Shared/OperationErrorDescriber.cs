using Microsoft.AspNetCore.Identity;
using System.Globalization;

namespace PPE.Model.Shared;

public class OperationErrorDescriber : IdentityErrorDescriber
{
    public override IdentityError ConcurrencyFailure()
    {
        return base.ConcurrencyFailure();
    }

    public override IdentityError DefaultError()
    {
        return base.DefaultError();
    }

    public override IdentityError DuplicateEmail(string email)
    {
        return base.DuplicateEmail(email);
    }

    public override IdentityError DuplicateRoleName(string role)
    {
        return base.DuplicateRoleName(role);
    }

    public override IdentityError DuplicateUserName(string userName)
    {
        return base.DuplicateUserName(userName);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override IdentityError InvalidEmail(string? email)
    {
        return base.InvalidEmail(email);
    }

    public override IdentityError InvalidRoleName(string? role)
    {
        return base.InvalidRoleName(role);
    }

    public override IdentityError InvalidToken()
    {
        return base.InvalidToken();
    }

    public override IdentityError InvalidUserName(string? userName)
    {
        return base.InvalidUserName(userName);
    }

    public override IdentityError LoginAlreadyAssociated()
    {
        return base.LoginAlreadyAssociated();
    }

    public override IdentityError PasswordMismatch()
    {
        return base.PasswordMismatch();
    }

    public override IdentityError PasswordRequiresDigit()
    {
        return base.PasswordRequiresDigit();
    }

    public override IdentityError PasswordRequiresLower()
    {
        return base.PasswordRequiresLower();
    }

    public override IdentityError PasswordRequiresNonAlphanumeric()
    {
        return base.PasswordRequiresNonAlphanumeric();
    }

    public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
    {
        return base.PasswordRequiresUniqueChars(uniqueChars);
    }

    public override IdentityError PasswordRequiresUpper()
    {
        return base.PasswordRequiresUpper();
    }

    public override IdentityError PasswordTooShort(int length)
    {
        return base.PasswordTooShort(length);
    }

    public override IdentityError RecoveryCodeRedemptionFailed()
    {
        return base.RecoveryCodeRedemptionFailed();
    }

    public override IdentityError UserAlreadyHasPassword()
    {
        return base.UserAlreadyHasPassword();
    }

    public override IdentityError UserAlreadyInRole(string role)
    {
        return base.UserAlreadyInRole(role);
    }

    public override IdentityError UserLockoutNotEnabled()
    {
        return base.UserLockoutNotEnabled();
    }

    public override IdentityError UserNotInRole(string role)
    {
        return base.UserNotInRole(role);
    }

    public IdentityError FileNotExists(string path)
    {
        return new IdentityError
        {
            Code = nameof(FileNotExists),
            Description = string.Format(CultureInfo.CurrentCulture, ErrorResources.FileNotExists, path)
        };
    }

    public IdentityError ExcelSheetNameNotExists(string sheetName)
    {
        return new IdentityError
        {
            Code = nameof(ExcelSheetNameNotExists),
            Description = string.Format(CultureInfo.CurrentCulture, ErrorResources.ExcelSheetNameNotExists, sheetName)
        };
    }
    /// <summary>
    /// 业务模型属性与Excel表的列数量不一致错误。
    /// </summary>
    /// <param name="sheetName">Excel 表名称</param>
    /// <returns></returns>
    public IdentityError ExcelColumnCountError(string sheetName)
    {
        return new IdentityError
        {
            Code = nameof(ExcelColumnCountError),
            Description = string.Format(CultureInfo.CurrentCulture, ErrorResources.ExcelColumnCountError, sheetName)
        };
    }

    public IdentityError ExcelSheetTitleInvalid(string sheetName, int columnIndex)
    {
        return new IdentityError
        {
            Code = nameof(ExcelSheetTitleInvalid),
            Description = string.Format(CultureInfo.CurrentCulture, ErrorResources.ExcelSheetTitleInvalid, sheetName, columnIndex)
        };
    }

    /// <summary>
    /// 删除记录时记录已经删除错误。
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public IdentityError AlreadyDeleteError(string content)
    {
        return new IdentityError
        {
            Code = nameof(AlreadyDeleteError),
            Description = string.Format(CultureInfo.CurrentCulture, ErrorResources.AlreadyDeleteError, content)
        };
    }

    /// <summary>
    /// 恢复或移除记录记录未删除错误。
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public IdentityError NotDeleteError(string content)
    {
        return new IdentityError
        {
            Code = nameof(NotDeleteError),
            Description = string.Format(CultureInfo.CurrentCulture, ErrorResources.NotDeleteError, content)
        };
    }
    /// <summary>
    /// 不能为空属性为空错误。
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public IdentityError CannotNullError(string content)
    {
        return new IdentityError
        {
            Code = nameof(CannotNullError),
            Description = string.Format(CultureInfo.CurrentCulture, ErrorResources.CannotNullError, content)
        };
    }

    /// <summary>
    /// 记录存在错误。
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public IdentityError DuplicateError(string content)
    {
        return new IdentityError
        {
            Code = nameof(DuplicateError),
            Description = string.Format(CultureInfo.CurrentCulture, ErrorResources.DuplicateError, content)
        };
    }

    /// <summary>
    /// 查询记录时记录不存在错误。
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public IdentityError NotExistsError(string content)
    {
        return new IdentityError
        {
            Code = nameof(NotExistsError),
            Description = string.Format(CultureInfo.CurrentCulture, ErrorResources.NotExistsError, content)
        };
    }

    /// <summary>
    /// 删除系统管理员时错误。
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="realName"></param>
    /// <returns></returns>
    public IdentityError AdministratorCannotDelete(string? userName, string? realName)
    {
        return new IdentityError
        {
            Code = nameof(AdministratorCannotDelete),
            Description = string.Format(CultureInfo.CurrentCulture, ErrorResources.AdministratorCannotDelete, userName, realName)
        };
    }

    /// <summary>
    /// 修改系统管理员账号时错误。
    /// </summary>
    /// <param name="userName"></param>
    /// <returns></returns>
    public IdentityError AdministratorCannotModifyUserName(string? userName)
    {

        return new IdentityError
        {
            Code = nameof(AdministratorCannotModifyUserName),
            Description = string.Format(CultureInfo.CurrentCulture, ErrorResources.AdministratorCannotModifyUserName, userName)
        };
    }

    /// <summary>
    /// 不能为空错误。
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public IdentityError InvalidError(string content)
    {
        return new IdentityError
        {
            Code = nameof(InvalidError),
            Description = string.Format(CultureInfo.CurrentCulture, ErrorResources.InvalidError, content)
        };
    }

    /// <summary>
    /// 角色不允许删除。
    /// </summary>
    /// <param name="role"></param>
    /// <returns></returns>
    public IdentityError RoleCannotDelete(string role)
    {
        return new IdentityError
        {
            Code = nameof(RoleCannotDelete),
            Description = string.Format(CultureInfo.CurrentCulture, ErrorResources.RoleCannotDelete, role)
        };
    }

    /// <summary>
    /// 角色未删除。
    /// </summary>
    /// <param name="role"></param>
    /// <returns></returns>
    public IdentityError RoleNotDeleted(string role)
    {
        return new IdentityError
        {
            Code = nameof(RoleNotDeleted),
            Description = string.Format(CultureInfo.CurrentCulture, ErrorResources.RoleNotDeleted, role)
        };
    }
}