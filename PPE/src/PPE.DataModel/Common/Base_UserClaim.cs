using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPE.DataModel;

[Table("Base_UserClaim")]
public class Base_UserClaim : IdentityUserClaim<string>
{
    [StringLength(36)]
    [ForeignKey(nameof(Base_User))]
    public override string UserId { get => base.UserId; set => base.UserId = value; }

    [StringLength(128)]
    public override string? ClaimType { get => base.ClaimType; set => base.ClaimType = value; }

    [StringLength(128)]
    public override string? ClaimValue { get => base.ClaimValue; set => base.ClaimValue = value; }
}
