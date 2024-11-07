using PPE.Model.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPE.DataModel;
/// <summary>
/// 课程内容
/// </summary>
[Table("Base_CourseContent")]
public class Base_CourseContent : BaseDataModel
{
    /// <summary>
    /// 目录主键
    /// </summary>
    /// <value></value>
    [StringLength(36)]
    public virtual string CourseCatalogueId { get; set; } = default!;

    /// <summary>
    /// 标题
    /// </summary>
    /// <value></value>
    [StringLength(128)]
    public virtual string? Title { get; set; }

    /// <summary>
    /// 标题标准值
    /// </summary>
    /// <value></value>
    [StringLength(128)]
    public virtual string? NormalizedTitle { get; set; }

    public virtual string? Content { get; set; }


    public override string ToString()
    {
        return Title ?? string.Empty;
    }
}