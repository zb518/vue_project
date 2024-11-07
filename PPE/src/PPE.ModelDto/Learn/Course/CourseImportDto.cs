using PPE.Model.Shared;
using System.ComponentModel.DataAnnotations;

namespace PPE.ModelDto;

/// <summary>
/// 课程导入导出信息
/// </summary>
[Sheet("课程")]
public class CourseImportDto
{

    /// <summary>
    /// 课程代码
    /// </summary>
    /// <value></value>
    [Display(Name = "代码", Order = 1)]
    public string? Code { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    /// <value></value>
    [Display(Name = "名称", Order = 2)]
    public string? Name { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    /// <value></value>
    [Display(Name = "地址", Order = 3)]
    public string? Url { get; set; }

    /// <summary>
    /// 说明
    /// </summary>
    /// <value></value>
    [Display(Name = "说明", Order = 4)]
    public string? Description { get; set; }
}