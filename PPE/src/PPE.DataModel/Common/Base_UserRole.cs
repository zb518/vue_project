using Microsoft.AspNetCore.Identity;
using PPE.Model.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPE.DataModel;

/// <summary>
/// 用户角色关系
/// </summary>
[Table("Base_UserRole")]
public class Base_UserRole : IdentityUserRole<string>
{
    [Key]
    [StringLength(36)]
    [ForeignKey(nameof(Base_User))]
    public override string UserId { get => base.UserId; set => base.UserId = value; }

    [Key]
    [StringLength(36)]
    [ForeignKey(nameof(Base_Role))]
    public override string RoleId { get => base.RoleId; set => base.RoleId = value; }

    /// <summary>
    /// 创建用户主键
    /// </summary>
    [StringLength(36)]
    public virtual string? CreateUserId { get; set; }

    /// <summary>
    /// 创建用户账号
    /// </summary>
    [StringLength(128)]
    public virtual string? CreateUserName { get; set; }

    /// <summary>
    /// 创建用户姓名
    /// </summary>
    [StringLength(128)]
    public virtual string? CreateRealName { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public virtual DateTime? CreateDate { get; set; }


    /// <summary>
    /// 设置创建用户信息、更新时间及乐观并发戳
    /// </summary>
    /// <param name="user">操作用户信息 <see cref="SignUser"/></param>
    public void SetCreate(SignUser? user = null)
    {
        CreateDate = DateTime.Now;
        if (user != null)
        {
            CreateUserId = user.Id;
            CreateUserName = user.UserName;
            CreateRealName = user.RealName;
        }
    }
}
