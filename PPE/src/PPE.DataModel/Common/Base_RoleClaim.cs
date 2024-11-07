using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPE.DataModel;

/// <summary>
/// 角色声明信息
/// </summary>
[Table("Base_RoleClaim")]
public class Base_RoleClaim : IdentityRoleClaim<string>
{
    [StringLength(36)]
    [ForeignKey(nameof(Base_Role))]
    public override string RoleId { get => base.RoleId; set => base.RoleId = value; }

    [StringLength(128)]
    public override string? ClaimType { get => base.ClaimType; set => base.ClaimType = value; }

    [StringLength(128)]
    public override string? ClaimValue { get => base.ClaimValue; set => base.ClaimValue = value; }
}
