using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPE.DataModel;

/// <summary>
/// 用户登录信息
/// </summary>
[Table("Base_UserLogin")]
public class Base_UserLogin : IdentityUserLogin<string>
{
    [StringLength(36)]
    [ForeignKey(nameof(Base_User))]
    public override string UserId { get => base.UserId; set => base.UserId = value; }

    [StringLength(128)]
    public override string LoginProvider { get => base.LoginProvider; set => base.LoginProvider = value; }

    [StringLength(128)]
    public override string ProviderKey { get => base.ProviderKey; set => base.ProviderKey = value; }

    [StringLength(128)]
    public override string? ProviderDisplayName { get => base.ProviderDisplayName; set => base.ProviderDisplayName = value; }
}
