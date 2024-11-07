using PPE.Model.Shared;
using System.ComponentModel.DataAnnotations;

namespace PPE.ModelDto;

/// <summary>
/// 专业导入导出信息
/// </summary>
[Sheet("专业")]
public class MajorImportDto
{
    /// <summary>
    /// 名称
    /// </summary>
    /// <value></value>
    [Display(Name = "名称", Order = 1)]
    public string? Name { get; set; }

    /// <summary>
    /// 代码
    /// </summary>
    /// <value></value>
    [Display(Name = "代码", Order = 2)]
    public string? Code { get; set; }

    /// <summary>
    /// 课程组
    /// </summary>
    /// <value></value>
    [Display(Name = "课程组", Order = 3)]
    public string? CurriculumGroup { get; set; }

    /// <summary>
    /// 层级
    /// </summary>
    /// <value></value>
    [Display(Name = "层级", Order = 4)]
    public string? Level { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <value></value>
    [Display(Name = "说明", Order = 5)]
    public string? Description { get; set; }
}
