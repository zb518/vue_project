using System.ComponentModel.DataAnnotations;

namespace PPE.ModelDto;

public class CourseContentEditDto
{
    public string? Id { get; set; }
    public string CourseCatalogueId { get; set; } = default!;

    [Display(Name = "目录内容")]
    [Required(ErrorMessage = "{0} 不能为空")]
    public string? CourseCatalogueContent { get; set; }


    /// <summary>
    /// 标题
    /// </summary>
    /// <value></value>
    [Display(Name = "标题")]
    public virtual string? Title { get; set; }


    [Display(Name = "内容", Order = 5)]
    public virtual string? Content { get; set; }
}