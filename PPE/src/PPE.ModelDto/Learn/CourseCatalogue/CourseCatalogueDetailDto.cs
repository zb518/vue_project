using PPE.Model.Shared;
using System.ComponentModel.DataAnnotations;

namespace PPE.ModelDto;

public class CourseCatalogueDetailDto : BaseDetailViewModel
{
    /// <summary>
    /// 课程主键
    /// </summary>
    /// <value></value>
    [Display(Order = 1)]
    public string CourseId { get; set; } = default!;

    /// <summary>
    /// 上级目录主键
    /// </summary>
    /// <value></value>
    [Display(Order = 2)]
    public string? ParentId { get; set; }

    /// <summary>
    /// 目录内容
    /// </summary>
    /// <value></value>
    [Display(Order = 3, Name = "目录内容")]
    public string? Content { get; set; }

    /// <summary>
    /// 目录内容标准值
    /// </summary>
    /// <value></value>
    [Display(Order = 4, Name = "目录内容标准值")]
    public string? NormalizedContent { get; set; }

    /// <summary>
    /// 排序代码
    /// </summary>
    /// <value></value>
    [Display(Order = 5, Name = "排序代码")]
    public int SortCode { get; set; }
}