using PPE.Model.Shared;
using System.ComponentModel.DataAnnotations;

namespace PPE.ModelDto;

/// <summary>
/// 课程目录导入导出信息
/// </summary>
[Sheet("课程目录")]
public class CourseCatalogueImportDto
{

    /// <summary>
    /// 课程名称
    /// </summary>
    /// <value></value>
    [Display(Name = "课程", Order = 1)]
    public string? Course { get; set; }

    /// <summary>
    /// 上级内容
    /// </summary>
    /// <value></value>
    [Display(Name = "上级内容", Order = 2)]
    public string? ParentName { get; set; }
    /// <summary>
    /// 内容
    /// </summary>
    /// <value></value>
    [Display(Name = "内容", Order = 3)]
    public string? Content { get; set; }

    /// <summary>
    /// 说明
    /// </summary>
    /// <value></value>
    [Display(Name = "说明", Order = 4)]
    public string? Description { get; set; }
}