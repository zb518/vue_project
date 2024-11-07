using System.ComponentModel.DataAnnotations;

namespace PPE.Model.Shared;

/// <summary>
/// 记录明细基类，字符串主键
/// </summary>
public class DataTablesBaseModel : DataTablesBaseModel<string>
{

}

/// <summary>
/// 记录明细基类，字符串主键
/// </summary>
/// <typeparam name="TKey"></typeparam>
public class DataTablesBaseModel<TKey> where TKey : IEquatable<TKey>
{
    [Key]
    [Display(Name = "主键", Order = 0)]
    public virtual TKey Id { get; set; } = default!;
}

/// <summary>
/// 记录明细基类，有操作用户信息，字符串主键
/// </summary>
public class DataTablesBaseModelWithEdit : DataTablesBaseModelWithEdit<string>
{

}

/// <summary>
/// 记录明细基类，有操作用户信息，泛型主键
/// </summary>
/// <typeparam name="TKey"></typeparam>
public class DataTablesBaseModelWithEdit<TKey> : DataTablesBaseModel<TKey> where TKey : IEquatable<TKey>
{
    /// <summary>
    /// 删除
    /// </summary>
    /// <value></value>
    [Display(Name = "删除", Order = 101)]
    [DataTablesColumn(Orderable = true)]
    public bool IsDeleted { get; set; }

    /// <summary>
    /// 说明
    /// </summary>
    /// <value></value>
    [Display(Name = "说明", Order = 102)]
    public string? Description { get; set; }

    /// <summary>
    /// 创建用户账号
    /// </summary>
    [Display(Name = "创建用户账号", Order = 103)]
    public virtual string? CreateUserName { get; set; }

    /// <summary>
    /// 创建用户姓名
    /// </summary>
    [Display(Name = "创建用户姓名", Order = 104)]
    public virtual string? CreateRealName { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [Display(Name = "创建时间", Order = 105)]
    [DataTablesColumn(Orderable = true)]
    public virtual DateTime? CreateDate { get; set; }

    /// <summary>
    /// 更新用户账号
    /// </summary>
    [Display(Name = "更新用户账号", Order = 106)]
    public virtual string? UpdateUserName { get; set; }

    /// <summary>
    /// 更新用户姓名
    /// </summary>
    [Display(Name = "更新用户姓名", Order = 107)]
    public virtual string? UpdateRealName { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [Display(Name = "更新时间", Order = 108)]
    [DataTablesColumn(Orderable = true)]
    public virtual DateTime? UpdateDate { get; set; }
}
