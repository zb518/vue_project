using PPE.Model.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPE.DataModel;
/// <summary>
/// 操作按钮信息
/// </summary>
[Table("Base_Button")]
public class Base_Button : BaseDataModel
{
    /// <summary>
    /// 系统菜单主键
    /// </summary>
    /// <value></value>
    [StringLength(36)]
    [ForeignKey(nameof(Base_Menu))]
    public virtual string MenuId { get; set; } = default!;

    /// <summary>
    /// 名称
    /// </summary>
    [StringLength(50)]
    public virtual string? Name { get; set; }

    /// <summary>
    /// 操作按钮组别
    /// </summary>
    [StringLength(50)]
    public virtual ButtonGroup ButtonGroup { get; set; }

    /// <summary>
    /// 操作按钮组别
    /// </summary>
    [StringLength(50)]
    public virtual ButtonType ButtonType { get; set; }

    /// <summary>
    /// 提示
    /// </summary>
    [StringLength(50)]
    public virtual string? Title { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [StringLength(20)]
    [RegularExpression(@"^[0-9]*$")]
    public virtual string? SortCode { get; set; }

    /// <summary>
    /// 右键
    /// </summary>
    /// <value></value>
    public virtual bool IsRight { get; set; }

    /// <summary>
    /// 样式
    /// </summary>
    [StringLength(50)]
    public virtual string? Css { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    [StringLength(128)]
    public virtual string? Icon { get; set; }

    /// <summary>
    /// JS 事件
    /// </summary>
    [StringLength(50)]
    public virtual string? JSEvent { get; set; }

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
    /// 资源地址
    /// </summary>
    [StringLength(255)]
    public virtual string? Url { get; set; }

    /// <summary>
    /// 资源地址标准值
    /// </summary>
    [StringLength(255)]
    public virtual string? NormalizedUrl { get; set; }



    public override string ToString()
    {
        return Name ?? string.Empty;
    }
}
