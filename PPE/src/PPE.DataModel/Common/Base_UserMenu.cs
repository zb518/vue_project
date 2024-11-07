using PPE.Model.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPE.DataModel;
/// <summary>
/// 用户系统菜单权限
/// </summary>
[Table("Base_UserMenu")]
public class Base_UserMenu
{
    /// <summary>
    /// 用户主键
    /// </summary>
    /// <value></value>
    [Key]
    [StringLength(36)]
    [ForeignKey(nameof(Base_User))]
    public virtual string UserId { get; set; } = default!;

    /// <summary>
    /// 系统菜单主键
    /// </summary>
    /// <value></value>
    [Key]
    [StringLength(36)]
    [ForeignKey(nameof(Base_Menu))]
    public virtual string MenuId { get; set; } = default!;

    /// <summary>
    /// 创建用户主键
    /// </summary>
    /// <value></value>		
    [StringLength(36)]
    public virtual string? CreateUserId { get; set; }

    /// <summary>
    /// 创建用户账号
    /// </summary>
    /// <value></value>
    [StringLength(128)]
    public virtual string? CreateUserName { get; set; }

    /// <summary>
    /// 创建用户姓名
    /// </summary>
    /// <value></value>
    [StringLength(128)]
    public virtual string? CreateRealName { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    /// <value></value>	
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