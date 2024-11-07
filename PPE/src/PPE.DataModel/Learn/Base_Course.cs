using PPE.Model.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPE.DataModel;

/// <summary>
/// 课程
/// </summary>
[Table("Base_Course")]
public class Base_Course : BaseDataModel
{

    /// <summary>
    /// 课程代码
    /// </summary>
    /// <value></value>
    [StringLength(10)]
    public virtual string? Code { get; set; }

    /// <summary>
    /// 代码标准值
    /// </summary>
    /// <value></value>
    [StringLength(10)]
    public virtual string? NormalizedCode { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    /// <value></value>
    [StringLength(100)]
    public virtual string? Name { get; set; }

    /// <summary>
    /// 名称索引
    /// </summary>
    /// <value></value>
    [StringLength(100)]
    public virtual string? NormalizedName { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    /// <value></value>
    [StringLength(255)]
    public virtual string? Url { get; set; }

    /// <summary>
    /// 地址标准值
    /// </summary>
    /// <value></value>
    [StringLength(255)]
    public virtual string? NormalizedUrl { get; set; }

    public override string ToString()
    {
        return Name ?? string.Empty;
    }
}