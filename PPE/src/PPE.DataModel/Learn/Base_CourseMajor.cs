using PPE.Model.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPE.DataModel;

/// <summary>
/// 专业课程关系
/// </summary>
[Table("Base_CourseMajor")]
public class Base_CourseMajor
{
    [Key]
    [StringLength(36)]
    [ForeignKey(nameof(Base_Course))]
    public virtual string CourseId { get; set; } = default!;

    [Key]
    [StringLength(36)]
    [ForeignKey(nameof(Base_Major))]
    public virtual string MajorId { get; set; } = default!;

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
    /// 设置添加用户信息及添加时间，包括导入
    /// </summary>
    /// <param name="user">操作用户信息 <see cref="SignUser"/></param>
    public virtual void SetCreate(SignUser? user = null)
    {
        if (user != null)
        {
            CreateUserId = user.Id;
            CreateUserName = user.UserName;
            CreateRealName = user.RealName;
        }
        CreateDate = DateTime.Now;
    }
}