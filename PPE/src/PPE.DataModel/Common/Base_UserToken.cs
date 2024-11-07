using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPE.DataModel;

/// <summary>
/// 用户令牌信息
/// </summary>
[Table("Base_UserToken")]
public class Base_UserToken : IdentityUserToken<string>
{
    [Key]
    [StringLength(36)]
    [ForeignKey(nameof(Base_User))]
    public override string UserId { get => base.UserId; set => base.UserId = value; }

    [StringLength(100)]
    public override string LoginProvider { get => base.LoginProvider; set => base.LoginProvider = value; }

    [StringLength(100)]
    public override string Name { get => base.Name; set => base.Name = value; }

    [StringLength(255)]
    public override string? Value { get => base.Value; set => base.Value = value; }
}
