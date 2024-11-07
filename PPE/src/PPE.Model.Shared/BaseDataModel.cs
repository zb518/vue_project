using System.ComponentModel.DataAnnotations;

namespace PPE.Model.Shared;

public class BaseDataModel : BaseDataModel<string>
{
    public BaseDataModel()
    {
        Id = Guid.NewGuid().ToString();
    }
    [StringLength(36)]
    public override string Id { get => base.Id; set => base.Id = value; }
}

public class BaseDataModel<TKey> where TKey : IEquatable<TKey>
{
    /// <summary>
    /// 主键
    /// </summary>
    /// <value></value>
    [Key]
    public virtual TKey Id { get; set; } = default!;

    /// <summary>
    /// 乐观并发戳
    /// </summary>
    /// <returns></returns>
    [StringLength(36)]
    public virtual string? ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// 删除标记
    /// </summary>
    /// <value></value>
    public virtual bool IsDeleted { get; set; }

    /// <summary>
    /// 说明
    /// </summary>
    /// <value></value>
    [StringLength(255)]
    public virtual string? Description { get; set; }

    /// <summary>
    /// 创建用户主键
    /// </summary>
    /// <value></value>
    [StringLength(36)]
    public virtual string? CreateUserId { get; set; }

    /// <summary>
    /// 创建用户账号
    /// </summary>
    /// <value></value>
    [StringLength(128)]
    public virtual string? CreateUserName { get; set; }

    /// <summary>
    /// 创建用户姓名
    /// </summary>
    /// <value></value>
    [StringLength(128)]
    public virtual string? CreateRealName { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    /// <value></value>
    public virtual DateTime? CreateDate { get; set; }

    /// <summary>
    /// 更新用户主键
    /// </summary>
    /// <value></value>
    [StringLength(36)]
    public virtual string? UpdateUserId { get; set; }

    /// <summary>
    /// 更新用户账号
    /// </summary>
    /// <value></value>
    [StringLength(128)]
    public virtual string? UpdateUserName { get; set; }

    /// <summary>
    /// 更新用户姓名
    /// </summary>
    /// <value></value>
    [StringLength(128)]
    public virtual string? UpdateRealName { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    /// <value></value>
    public virtual DateTime? UpdateDate { get; set; }

    /// <summary>
    /// 设置创建用户信息信、更新时间及乐观并发戳
    /// </summary>
    /// <param name="user">操作用户信息 <see cref="SignUser"/></param>
    public virtual void SetCreate(SignUser? user = null)
    {
        ConcurrencyStamp ??= Guid.NewGuid().ToString();
        CreateDate = DateTime.Now;
        if (user != null)
        {
            CreateUserId = user.Id;
            CreateUserName = user.UserName;
            CreateRealName = user.RealName;
        }
    }

    /// <summary>
    /// 设置更新用户信息信、更新时间及乐观并发戳
    /// </summary>
    /// <param name="user">操作用户信息 <see cref="SignUser"/></param>
    public virtual void SetUpdate(SignUser? user = null)
    {
        ConcurrencyStamp = Guid.NewGuid().ToString();
        UpdateDate = DateTime.Now;
        if (user != null)
        {
            UpdateUserId = user.Id;
            UpdateUserName = user.UserName;
            UpdateRealName = user.RealName;
        }
    }
}