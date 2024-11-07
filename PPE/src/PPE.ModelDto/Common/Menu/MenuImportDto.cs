using PPE.Model.Shared;
using System.ComponentModel.DataAnnotations;

namespace PPE.ModelDto;

/// <summary>
/// 
/// </summary>
[Sheet("系统菜单")]
public class MenuImportDto
{
    /// <summary>
    /// 上级名称
    /// </summary>
    /// <value></value>
    [Display(Name = "上级名称", Order = 1)]
    public string? ParentName { get; set; }

    /// <summary>
    /// 菜单名称
    /// </summary>
    /// <value></value>
    [Display(Name = "名称", Order = 2)]
    public string? Name { get; set; }

    /// <summary>
    /// 提示
    /// </summary>
    [Display(Name = "提示", Order = 3)]
    public string? Title { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    /// <value></value>
    [Display(Name = "图标", Order = 4)]
    public string? Icon { get; set; }

    /// <summary>
    /// 区域
    /// </summary>
    /// <value></value>
    [Display(Name = "区域", Order = 5)]
    public string? Area { get; set; }

    /// <summary>
    /// 页面地址
    /// </summary>
    /// <value></value>
    [Display(Name = "页面", Order = 6)]
    public string? Page { get; set; }

    /// <summary>
    /// 说明
    /// </summary>
    /// <value></value>
    [Display(Name = "说明", Order = 7)]
    public string? Description { get; set; }

    public override string ToString()
    {
        return Name ?? string.Empty;
    }
}
