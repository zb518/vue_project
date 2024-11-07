using PPE.Model.Shared;
using System.ComponentModel.DataAnnotations;

namespace PPE.ModelDto;

public class OperationButtonDto
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Title { get; set; }
    public string? MenuId { get; set; }
    public string? Icon { get; set; }
    public string? Css { get; set; }

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
    /// 右键
    /// </summary>
    /// <value></value>
    public bool IsRight { get; set; }
    public string? JSEvent { get; set; }
    public string? Area { get; set; }
    public string? NormalizedArea { get; set; }
    public string? Url { get; set; }
    public string? NormalizedUrl { get; set; }
    public string? Description { get; set; }
    public string? SortCode { get; set; }

    public override string ToString()
    {
        return Name ?? string.Empty;
    }
}