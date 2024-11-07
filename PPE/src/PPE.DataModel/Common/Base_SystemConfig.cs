using PPE.Model.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPE.DataModel;
/// <summary>
/// 系统配置
/// </summary>
[Table("Base_SystemConfig")]
public class Base_SystemConfig : BaseDataModel
{
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
    /// 配置值
    /// </summary>
    /// <value></value>
    [StringLength(255)]
    public virtual string? Value { get; set; }

    public override string ToString()
    {
        return Name ?? string.Empty;
    }
}