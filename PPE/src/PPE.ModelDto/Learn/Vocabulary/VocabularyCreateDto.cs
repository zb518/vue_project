using System.ComponentModel.DataAnnotations;

namespace PPE.ModelDto;


public class VocabularyCreateDto
{
    [Display(Name = "词条")]
    [StringLength(128)]
    [Required(ErrorMessage = "{0} 不能为空")]
    public string? Word { get; set; }

    [Display(Name = "音标")]
    [StringLength(128)]
    public string? PhoneticSymbol { get; set; }

    /// <summary>
    /// 
    /// /// </summary>
    /// <value></value>
    [Display(Name = "解释")]
    public string? Expressions { get; set; }

    [Display(Name = "说明")]
    [StringLength(255)]
    public string? Description { get; set; }

}