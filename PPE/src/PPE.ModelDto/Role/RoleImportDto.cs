using PPE.Model.Shared;
using System.ComponentModel.DataAnnotations;

namespace PPE.ModelDto;
/// <summary>
/// 
/// </summary>
[Sheet("角色")]
public class RoleImportDto
{
    /// <summary>
    /// 名称
    /// </summary>
    /// <value></value>
    [Display(Name = "名称", Order = 1)]
    public string? Name { get; set; }

    /// <summary>
    /// 说明
    /// </summary>
    /// <value></value>
    [Display(Name = "说明", Order = 2)]
    public string? Description { get; set; }
}
