using PPE.Model.Shared;
using System.ComponentModel.DataAnnotations;

namespace PPE.ModelDto;
/// <summary>
/// 专业明细信息
/// </summary>
public class MajorDetailDto : BaseDetailViewModel
{
    /// <summary>
    /// 代码
    /// </summary>
    /// <value></value>
    [Display(Name = "代码", Order = 1)]
    public string? Code { get; set; }

    /// <summary>
    /// 代码
    /// </summary>
    /// <value></value>
    [Display(Name = "代码标准值", Order = 2)]
    public string? NormalizedCode { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    /// <value></value>
    [Display(Name = "主键", Order = 3)]
    public string? Name { get; set; }

    /// <summary>
    /// 名称标准值
    /// </summary>
    /// <value></value>
    [Display(Order = 4)]
    public string? NormalizedName { get; set; }

    /// <summary>
    /// 课程组
    /// </summary>
    /// <value></value>
    [Display(Name = "课程组", Order = 5)]
    public string? CurriculumGroup { get; set; }

    /// <summary>
    /// 层级
    /// </summary>
    /// <value></value>
    [Display(Name = "层级", Order = 6)]
    public string? Level { get; set; }
}