using PPE.DataModel;
using System.ComponentModel.DataAnnotations;

namespace PPE.ModelDto;
/// <summary>
/// 创建专业业务信息
/// </summary>
public class MajorCreateDto
{
    /// <summary>
    /// 代码
    /// </summary>
    /// <value></value>
    [Display(Name = "代码")]
    [StringLength(10)]
    [Required(ErrorMessage = "{0} 不能为空")]
    public string? Code { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    /// <value></value>
    [Display(Name = "名称")]
    [StringLength(128)]
    [Required(ErrorMessage = "{0} 不能为空")]
    public string? Name { get; set; }


    /// <summary>
    /// 课程组
    /// </summary>
    /// <value></value>
    [Display(Name = "课程组")]
    [StringLength(50)]
    public string? CurriculumGroup { get; set; }

    /// <summary>
    /// 层级
    /// </summary>
    /// <value></value>
    [Display(Name = "层级")]
    [StringLength(50)]
    [Required(ErrorMessage = "{0} 不能为空")]
    public string? Level { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <value></value>
    [Display(Name = "说明")]
    [StringLength(255)]
    public string? Description { get; set; }



    public Base_Major ConverToEntity()
    {
        return new Base_Major
        {
            Name = Name,
            Code = Code,
            Description = Description,
        };
    }
}


public class MajorEditDto : MajorCreateDto
{
    public string Id { get; set; } = default!;
}
