using PPE.Model.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPE.DataModel;
/// <summary>
/// 目录信息
/// </summary>
[Table("Base_CourseCatalogue")]
public class Base_CourseCatalogue : BaseDataModel
{
    /// <summary>
    /// 课程主键
    /// </summary>
    /// <value></value>
    [StringLength(36)]
    [ForeignKey(nameof(Base_Course))]
    public virtual string CourseId { get; set; } = default!;

    /// <summary>
    /// 上级目录主键
    /// </summary>
    /// <value></value>
    [StringLength(36)]
    public virtual string? ParentId { get; set; }

    /// <summary>
    /// 目录内容
    /// </summary>
    /// <value></value>
    [StringLength(128)]
    public virtual string? Content { get; set; }

    /// <summary>
    /// 目录内容标准值
    /// </summary>
    /// <value></value>
    [StringLength(128)]
    public virtual string? NormalizedContent { get; set; }

    /// <summary>
    /// 排序代码
    /// </summary>
    /// <value></value>
    public virtual int SortCode { get; set; }

    // /// <summary>
    // /// 地址
    // /// </summary>
    // /// <value></value>
    // [StringLength(255)]
    // public virtual string? Url { get; set; }

    // /// <summary>
    // /// 地址标准值
    // /// </summary>
    // /// <value></value>
    // [StringLength(255)]
    // public virtual string? NormalizedUrl { get; set; }

    public override string ToString()
    {
        return Content ?? string.Empty;
    }
}