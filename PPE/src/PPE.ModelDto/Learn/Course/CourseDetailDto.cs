using PPE.Model.Shared;
using System.ComponentModel.DataAnnotations;

namespace PPE.ModelDto;
/// <summary>
/// 课程详细信息
/// </summary>
public class CourseDetailDto : BaseDetailViewModel
{
    /// <summary>
    /// 代码
    /// </summary>
    /// <value></value>
    [Display(Name = "代码")]
    public string? Code { get; set; }

    /// <summary>
    /// 代码标准值
    /// </summary>
    /// <value></value>
    [Display(Name = "代码标准值")]
    public string? NormalizedCode { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    /// <value></value>
    [Display(Name = "名称")]
    public string? Name { get; set; }

    /// <summary>
    /// 名称标准值
    /// </summary>
    /// <value></value>
    [Display(Name = "名称标准值")]
    public string? NormalizedName { get; set; }

    /// <summary>
    /// 专业主键
    /// </summary>
    /// <value></value>
    [Display(Name = "专业主键")]
    public string? MajorId { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    /// <value></value>
    [Display(Name = "地址")]
    public string? Url { get; set; }

    /// <summary>
    /// 地址标准值
    /// </summary>
    /// <value></value>
    [Display(Name = "地址标准值")]
    public string? NormalizedUrl { get; set; }
}
