using PPE.Model.Shared;
using System.ComponentModel.DataAnnotations;

namespace PPE.ModelDto;

public class RoleListDto : BaseDetailViewModel
{
    /// <summary>
    /// 名称
    /// </summary>
    /// <value></value>
    [Display(Name = "名称", Order = 1)]
    public string? Name { get; set; }
    /// <summary>
    /// 名称标准值
    /// </summary>
    /// <value></value>
    [Display(Name = "名称标准值", Order = 2)]
    public string? NormalizedName { get; set; }
}