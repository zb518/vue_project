using System.ComponentModel.DataAnnotations;
using PPE.Model.Shared;

namespace PPE.ModelDto;

public class MenuDetailDto : BaseDetailViewModel
{
    /// <summary>
    /// 上级主键
    /// </summary>
    /// <value></value>
    [Display(Name = "上级主键", Order = 1)]
    public virtual string ParentId { get; set; } = default!;

    /// <summary>
    /// 名称
    /// </summary>
    [Display(Name = "名称", Order = 2)]
    [DataTablesColumn(Searchable = true, Orderable = true, Visible = true)]
    public virtual string? Name { get; set; }

    /// <summary>
    /// 名称标准值
    /// </summary>
    [Display(Name = "名称标准值", Order = 3)]
    [DataTablesColumn(Searchable = true, Orderable = true)]
    public virtual string? NormalizedName { get; set; }

    /// <summary>
    /// 提示
    /// </summary>
    [Display(Name = "提示", Order = 4)]
    [DataTablesColumn(Visible = true)]
    public virtual string? Title { get; set; }

    /// <summary>
    /// 级数
    /// </summary>
    /// <value></value>
    [Display(Name = "级数", Order = 5)]
    [DataTablesColumn(Searchable = false, Orderable = true, Visible = true)]
    public virtual int Level { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [Display(Name = "排序", Order = 6)]
    [DataTablesColumn(Searchable = false, Orderable = true, Visible = true)]
    public virtual string? SortCode { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    [Display(Name = "图标", Order = 7)]
    [DataTablesColumn(Visible = true)]
    public virtual string? Icon { get; set; }

    /// <summary>
    /// 区域
    /// </summary>
    /// <value></value>
    [Display(Name = "区域", Order = 8)]
    [DataTablesColumn(Searchable = false, Orderable = true, Visible = true)]
    public string? Area { get; set; }

    /// <summary>
    /// 区域标准值
    /// </summary>
    /// <value></value>
    [Display(Name = "区域标准值", Order = 9)]
    [DataTablesColumn(Searchable = false, Orderable = true)]
    public string? NormalizedArea { get; set; }

    /// <summary>
    /// 页面地址
    /// </summary>
    [Display(Name = "页面", Order = 10)]
    [DataTablesColumn(Searchable = true, Orderable = true, Visible = true)]
    public virtual string? Page { get; set; }

    /// <summary>
    /// 页面地址标准值
    /// </summary>
    [Display(Name = "页面地址标准值", Order = 11)]
    [DataTablesColumn(Searchable = false, Orderable = true)]
    public virtual string? NormalizedPage { get; set; }
}