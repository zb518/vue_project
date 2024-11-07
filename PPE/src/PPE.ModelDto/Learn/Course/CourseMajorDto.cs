using PPE.Model.Shared;
using System.ComponentModel.DataAnnotations;

namespace PPE.ModelDto;
/// <summary>
/// 专业课程关系信息，以代码关联
/// </summary>
[Sheet("课程专业关系")]
public class CourseMajorDto
{
    /// <summary>
    /// 课程代码
    /// </summary>
    /// <value></value>
    [Display(Name = "课程代码", Order = 1)]
    public string? CourseCode { get; set; }

    /// <summary>
    /// 专业代码
    /// </summary>
    /// <value></value>
    [Display(Name = "专业代码", Order = 2)]
    public string? MajorCode { get; set; }
}