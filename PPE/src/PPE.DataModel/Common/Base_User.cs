using Microsoft.AspNetCore.Identity;
using PPE.Model.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPE.DataModel;

/// <summary>
/// 用户信息
/// </summary>
[Table("Base_User")]
public class Base_User : IdentityUser
{
    public Base_User() : base()
    {
    }

    public Base_User(string userName) : base(userName)
    {
    }

    [Key]
    [StringLength(36)]
    public override string Id { get => base.Id; set => base.Id = value; }

    [StringLength(128)]
    public override string? UserName { get => base.UserName; set => base.UserName = value; }


    [StringLength(128)]
    public override string? NormalizedUserName { get => base.NormalizedUserName; set => base.NormalizedUserName = value; }

    /// <summary>
    /// 姓名
    /// </summary>
    [StringLength(128)]
    [PersonalData]
    public virtual string? RealName { get; set; }

    [StringLength(128)]
    public override string? PasswordHash { get => base.PasswordHash; set => base.PasswordHash = value; }

    [StringLength(36)]
    public override string? SecurityStamp { get => base.SecurityStamp; set => base.SecurityStamp = value; }

    [StringLength(36)]
    public override string? ConcurrencyStamp { get => base.ConcurrencyStamp; set => base.ConcurrencyStamp = value; }


    [StringLength(128)]
    public override string? Email { get => base.Email; set => base.Email = value; }


    [StringLength(128)]
    public override string? NormalizedEmail { get => base.NormalizedEmail; set => base.NormalizedEmail = value; }


    [StringLength(128)]
    public override string? PhoneNumber { get => base.PhoneNumber; set => base.PhoneNumber = value; }

    /// <summary>
    /// 是否为管理员
    /// </summary>
    public virtual bool IsAdministrator { get; set; }

    /// <summary>
    /// 登录次数
    /// </summary>
    public virtual int AccessCount { get; set; }

    /// <summary>
    /// 最后登录IP地址
    /// </summary>
    [StringLength(48)]
    public virtual string? AccessIP { get; set; }

    /// <summary>
    /// 首次登录
    /// </summary>
    public virtual DateTime? FirstAccess { get; set; }

    /// <summary>
    /// 上次登录
    /// </summary>
    public virtual DateTime? PreviousAccess { get; set; }

    /// <summary>
    /// 最后登录
    /// </summary>
    public virtual DateTime? LastAccess { get; set; }

    /// <summary>
    /// 是否删除
    /// </summary>
    public virtual bool IsDeleted { get; set; }

    /// <summary>
    /// 说明
    /// </summary>
    [StringLength(255)]
    public virtual string? Description { get; set; }

    /// <summary>
    /// 创建用户主键
    /// </summary>
    [StringLength(36)]
    public virtual string? CreateUserId { get; set; }

    /// <summary>
    /// 创建用户账号
    /// </summary>
    [StringLength(128)]
    public virtual string? CreateUserName { get; set; }

    /// <summary>
    /// 创建用户姓名
    /// </summary>
    [StringLength(128)]
    public virtual string? CreateRealName { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public virtual DateTime? CreateDate { get; set; }

    /// <summary>
    /// 更新用户主键
    /// </summary>
    [StringLength(36)]
    public virtual string? UpdateUserId { get; set; }

    /// <summary>
    /// 更新用户账号
    /// </summary>
    [StringLength(128)]
    public virtual string? UpdateUserName { get; set; }

    /// <summary>
    /// 更新用户姓名
    /// </summary>
    [StringLength(128)]
    public virtual string? UpdateRealName { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public virtual DateTime? UpdateDate { get; set; }

    /// <summary>
    /// 设置创建用户信息、更新时间及乐观并发戳
    /// </summary>
    /// <param name="user">操作用户信息 <see cref="SignUser"/></param>
    public void SetCreate(SignUser? user = null)
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
    /// 设置更新用户信息、更新时间及乐观并发戳
    /// </summary>
    /// <param name="user">操作用户信息 <see cref="SignUser"/></param>
    public void SetUpdate(SignUser? user = null)
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