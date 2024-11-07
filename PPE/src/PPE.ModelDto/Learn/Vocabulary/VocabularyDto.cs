using PPE.Model.Shared;
using System.ComponentModel.DataAnnotations;

namespace PPE.ModelDto;

public class VocabularyDetailDto : BaseDetailViewModel
{
    [StringLength(128)]
    public string? Word { get; set; }

    [StringLength(128)]
    public string? NormalizedWord { get; set; }

    [StringLength(128)]
    public string? PhoneticSymbol { get; set; }
    /// <summary>
    /// 
    /// </summary>
    /// <value></value>
    public string? Expressions { get; set; }
}