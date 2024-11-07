using PPE.Model.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPE.DataModel;
/// <summary>
/// 专业信息
/// </summary>
[Table("Base_Major")]
public class Base_Major : BaseDataModel
{
    /// <summary>
    /// 专业代码
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
    /// 专业名称
    /// </summary>
    /// <value></value>
    [StringLength(128)]
    public virtual string? Name { get; set; }

    /// <summary>
    /// 专业名称标准值
    /// </summary>
    /// <value></value>
    [StringLength(128)]
    public virtual string? NormalizedName { get; set; }

    /// <summary>
    /// 课程组
    /// </summary>
    /// <value></value>
    [StringLength(50)]
    public virtual string? CurriculumGroup { get; set; }

    /// <summary>
    /// 层次
    /// </summary>
    /// <value></value>
    [StringLength(50)]
    public virtual string? Level { get; set; }

    public override string ToString()
    {
        return Name ?? string.Empty;
    }
}