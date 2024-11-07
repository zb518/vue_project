using PPE.Model.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPE.DataModel;
/// <summary>
/// 系统菜单
/// </summary>
[Table("Base_Menu")]
public class Base_Menu : BaseDataModel
{
    /// <summary>
    /// 上级主键
    /// </summary>
    /// <value></value>
    [StringLength(36)]
    public virtual string ParentId { get; set; } = default!;

    /// <summary>
    /// 名称
    /// </summary>
    [StringLength(50)]
    public virtual string? Name { get; set; }

    /// <summary>
    /// 名称标准值
    /// </summary>
    [StringLength(50)]
    public virtual string? NormalizedName { get; set; }

    /// <summary>
    /// 提示
    /// </summary>
    [StringLength(50)]
    public virtual string? Title { get; set; }

    /// <summary>
    /// 级数
    /// </summary>
    /// <value></value>
    public virtual int Level { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [StringLength(20)]
    public virtual string? SortCode { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    [StringLength(255)]
    public virtual string? Icon { get; set; }

    /// <summary>
    /// 区域
    /// </summary>
    /// <value></value>
    [StringLength(50)]
    public string? Area { get; set; }

    /// <summary>
    /// 区域标准值
    /// </summary>
    /// <value></value>
    [StringLength(50)]
    public string? NormalizedArea { get; set; }

    /// <summary>
    /// 页面地址
    /// </summary>
    [StringLength(128)]
    public virtual string? Page { get; set; }

    /// <summary>
    /// 页面地址标准值
    /// </summary>
    [StringLength(128)]
    public virtual string? NormalizedPage { get; set; }


    public override string ToString()
    {
        return Name ?? string.Empty;
    }
}