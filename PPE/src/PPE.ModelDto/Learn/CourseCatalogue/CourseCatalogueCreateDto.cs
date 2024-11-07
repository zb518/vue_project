using System.ComponentModel.DataAnnotations;

namespace PPE.ModelDto;

public class CourseCatalogueCreateDto
{
    /// <summary>
    /// 课程主键
    /// </summary>
    /// <value></value>
    [StringLength(36)]
    [Display(Name = "课程")]
    [Required(ErrorMessage = "{0} 不能为空")]
    public string? CourseId { get; set; }

    /// <summary>
    /// 上级目录主键
    /// </summary>
    /// <value></value>
    [StringLength(36)]
    [Display(Name = "上级主键")]
    //[Required(ErrorMessage = "{0} 不能为空")]
    public string? ParentId { get; set; }

    /// <summary>
    /// 上级目录
    /// </summary>
    /// <value></value>
    [Display(Name = "上级目录")]
    //[Required(ErrorMessage = "{0} 不能为空")]
    public string? ParentName { get; set; }

    /// <summary>
    /// 目录内容
    /// </summary>
    /// <value></value>
    [StringLength(128)]
    [Display(Name = "目录内容")]
    [Required(ErrorMessage = "{0} 不能为空")]
    public string? Content { get; set; }

    // /// <summary>
    // /// 地址
    // /// </summary>
    // /// <value></value>
    // [StringLength(255)]
    // [Display(Name = "地址")]
    // [Required(ErrorMessage = "{0} 不能为空")]
    // public string? Url { get; set; }

    /// <summary>
    /// 说明
    /// </summary>
    /// <value></value>
    [StringLength(255)]
    [Display(Name = "说明")]
    public string? Description { get; set; }
}

/// <summary>
/// 
/// </summary>
public class CourseCatalogueEditDto : CourseCatalogueCreateDto
{
    public string Id { get; set; } = default!;

    /// <summary>
    /// 排序代码
    /// </summary>
    /// <value></value>
    [StringLength(20)]
    [Display(Name = "排序")]
    [Required(ErrorMessage = "{0} 不能为空")]
    public string? SortCode { get; set; }
}
