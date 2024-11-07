using System.ComponentModel.DataAnnotations;

namespace PPE.Model.Shared;
/// <summary>
/// 记录明细视图基类——字符串主键
/// </summary>
public class BaseDetailViewModel : BaseDetailViewModel<string>
{

}
/// <summary>
/// 记录明细视图基类——泛型主键
/// </summary>
/// <typeparam name="TKey"></typeparam>
public class BaseDetailViewModel<TKey> where TKey : IEquatable<TKey>
{
    /// <summary>
    /// 主键
    /// </summary>
    /// <value></value>
    [Key]
    [Display(Name = "主键", Order = 0)]
    public virtual TKey? Id { get; set; }

    /// <summary>
    /// 删除
    /// </summary>
    /// <value></value>
    [Display(Order = 101, Name = "删除")]
    [DataTablesColumn(Orderable = true, Visible = true)]
    public virtual bool IsDeleted { get; set; }
    /// <summary>
    /// 说明
    /// </summary>
    /// <value></value>
    [Display(Order = 102, Name = "说明")]
    [DataTablesColumn(Orderable = false, Visible = true)]
    public virtual string? Description { get; set; }
    /// <summary>
    /// 创建用户账号
    /// </summary>
    /// <value></value>
    [Display(Order = 103, Name = "创建用户账号")]
    [DataTablesColumn(Orderable = false, Visible = false)]
    public virtual string? CreateUserName { get; set; }
    /// <summary>
    /// 创建用户姓名
    /// </summary>
    /// <value></value>
    [Display(Order = 104, Name = "创建用户姓名")]
    [DataTablesColumn(Orderable = false, Visible = false)]
    public virtual string? CreateRealName { get; set; }
    /// <summary>
    /// 创建时间
    /// </summary>
    /// <value></value>
    [Display(Order = 105, Name = "创建时间")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
    [DataTablesColumn(Orderable = true, Visible = false)]
    public virtual DateTime? CreateDate { get; set; }
    /// <summary>
    /// 更新用户账号
    /// </summary>
    /// <value></value>
    [Display(Order = 106, Name = "更新用户账号")]
    [DataTablesColumn(Orderable = false, Visible = false)]
    public virtual string? UpdateUserName { get; set; }
    /// <summary>
    /// 更新用户姓名
    /// </summary>
    /// <value></value>
    [Display(Order = 107, Name = "更新用户姓名")]
    [DataTablesColumn(Orderable = false, Visible = false)]
    public virtual string? UpdateRealName { get; set; }
    /// <summary>
    /// 更新时间
    /// </summary>
    /// <value></value>
    [Display(Order = 108, Name = "更新时间")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
    [DataTablesColumn(Orderable = true, Visible = false)]
    public virtual DateTime? UpdateDate { get; set; }

}