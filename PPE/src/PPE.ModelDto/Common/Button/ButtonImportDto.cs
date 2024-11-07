using PPE.Model.Shared;
using System.ComponentModel.DataAnnotations;

namespace PPE.ModelDto;

/// <summary>
/// 操作按钮导入导出信息
/// </summary>
[Sheet("操作按钮")]
public class ButtonImportDto
{
    /// <summary>
    /// 系统菜单名称
    /// </summary>
    [Display(Name = "菜单名称", Order = 0)]
    public string MenuName { get; set; } = default!;

    /// <summary>
    /// 名称
    /// </summary>
    [Display(Name = "名称", Order = 1)]
    public string? Name { get; set; }

    /// <summary>
    /// 分组代码
    /// </summary>
    [Display(Name = "分组", Order = 2)]
    public ButtonGroup ButtonGroup { get; set; }

    /// <summary>
    /// 类型
    /// </summary>
    /// <value></value>
    [Display(Name = "类型", Order = 3)]
    public ButtonType ButtonType { get; set; }

    /// <summary>
    /// 提示
    /// </summary>
    [Display(Name = "提示", Order = 5)]
    public string? Title { get; set; }

    /// <summary>
    /// 样式
    /// </summary>
    [Display(Name = "样式", Order = 6)]
    public string? Css { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    [Display(Name = "图标", Order = 7)]
    public string? Icon { get; set; }

    /// <summary>
    /// JS 事件
    /// </summary>
    [Display(Name = "JS事件", Order = 8)]
    public string? JSEvent { get; set; }

    /// <summary>
    /// 右键
    /// </summary>
    /// <value></value>
    [Display(Name = "右键", Order = 9)]
    public bool IsRight { get; set; }

    /// <summary>
    /// 区域
    /// </summary>
    /// <value></value>
    [Display(Name = "区域", Order = 10)]
    public string? Area { get; set; }

    /// <summary>
    /// 资源地址
    /// </summary>
    [Display(Name = "操作", Order = 11)]
    public string? Url { get; set; }

    /// <summary>
    /// 说明
    /// </summary>
    [Display(Name = "说明", Order = 12)]
    public string? Description { get; set; }



    public override string ToString()
    {
        return Name ?? string.Empty;
    }
}