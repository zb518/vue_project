using PPE.Model.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPE.DataModel;

/// <summary>
/// 词汇表
/// </summary>
[Table("Base_Vocabulary")]
public class Base_Vocabulary : BaseDataModel
{
    [StringLength(128)]
    public virtual string? Word { get; set; }

    [StringLength(128)]
    public virtual string? NormalizedWord { get; set; }

    [StringLength(128)]
    public virtual string? PhoneticSymbol { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <value></value>
    public virtual string? Expressions { get; set; }

    public override string ToString()
    {
        return Word ?? string.Empty;
    }
}
