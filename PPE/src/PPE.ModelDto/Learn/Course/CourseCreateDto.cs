using PPE.DataModel;
using System.ComponentModel.DataAnnotations;

namespace PPE.ModelDto;
/// <summary>
/// 创建课程业务信息
/// </summary>
public class CourseCreateDto
{
    /// <summary>
    /// 课程代码
    /// </summary>
    /// <value></value>
    [Display(Name = "代码", Order = 1)]
    [StringLength(10)]
    [Required(ErrorMessage = "{0} 不能为空")]
    public string? Code { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    /// <value></value>
    [Display(Name = "名称", Order = 2)]
    [StringLength(128)]
    [Required(ErrorMessage = "{0} 不能为空")]
    public string? Name { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    /// <value></value>
    [Display(Name = "地址", Order = 3)]
    [StringLength(255)]
    [Required(ErrorMessage = "{0} 不能为空")]
    public string? Url { get; set; } = "/LearnAdmin/Course/";

    /// <summary>
    /// 说明
    /// </summary>
    /// <value></value>
    [Display(Name = "说明", Order = 4)]
    public string? Description { get; set; }

    public Base_Course ConverToEntity()
    {
        return new Base_Course
        {
            Code = Code,
            Name = Name,
            Url = Url,
            Description = Description,
        };
    }
}

public class CourseEditDto : CourseCreateDto
{
    public string Id { get; set; } = default!;

    public void ConvertoModel(Base_Course course)
    {
        Id = course.Id;
        Code = course.Code;
        Name = course.Name;
        Description = course.Description;
        Url = course.Url;
    }
}
