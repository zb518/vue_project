using System.ComponentModel.DataAnnotations;
using PPE.Model.Shared;

namespace PPE.ModelDto;
/// <summary>
/// 
/// </summary>
public class CourseContentDetailDto : BaseDetailViewModel
{
    /// <summary>
    /// 目录主键
    /// </summary>
    /// <value></value>
    [Display(Order = 2)]
    public virtual string? CourseCatalogueId { get; set; }

    /// <summary>
    /// 标题
    /// </summary>
    /// <value></value>
    [Display(Name = "标题", Order = 3)]
    public virtual string? Title { get; set; }

    [Display(Order = 4)]
    public virtual string? NormalizedTitle { get; set; }

    [Display(Name = "内容", Order = 5)]
    public virtual string? Content { get; set; }
}