using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPE.DataModel;
/// <summary>
/// 登录日志
/// </summary>
[Table("Base_SignLog")]
public class Base_SignLog
{
    /// <summary>
    /// 
    /// </summary>
    /// <value></value>
    [Key]
    public virtual long Id { get; set; }
    /// <summary>
    /// 用户主键
    /// </summary>
    [StringLength(36)]
    public virtual string? UserId { get; set; }

    /// <summary>
    /// 账号
    /// </summary>
    [StringLength(250)]
    public virtual string? UserName { get; set; }

    /// <summary>
    /// 姓名
    /// </summary>
    [StringLength(250)]
    public virtual string? RealName { get; set; }

    /// <summary>
    /// 登录IP地址
    /// </summary>
    [StringLength(48)]
    public virtual string? LoginIP { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [StringLength(250)]
    public virtual string? Description { get; set; }

    /// <summary>
    /// 登录时间
    /// </summary>
    public virtual DateTime? LoginDate { get; set; } = DateTime.Now;

    /// <summary>
    /// 登录结果
    /// </summary>
    public virtual bool LoginResult { get; set; }

    /// <summary>
    /// 退出时间
    /// </summary>
    public virtual DateTime? LogoutTime { get; set; }

    public override string ToString()
    {
        return UserName ?? string.Empty;
    }
}
