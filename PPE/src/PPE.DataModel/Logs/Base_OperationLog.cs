using PPE.Model.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace PPE.DataModel;

/// <summary>
/// 操作日志数据信息
/// </summary>
[Table("Base_OperationLog")]
public class Base_OperationLog
{
    public Base_OperationLog()
    {
    }

    public Base_OperationLog(string? entityName, string? tableName, OperationLogType logType, string? userId, string? userName, string? realName, string? operateIP, DateTime? operateDate)
    {
        EntityName = entityName;
        TableName = tableName;
        OperationLogType = logType;
        UserId = userId;
        UserName = userName;
        RealName = realName;
        OperateIP = operateIP;
        OperateDate = operateDate;
    }

    [Key]
    public virtual ulong Id { get; set; }

    /// <summary>
    /// 实体名称
    /// </summary>
    [StringLength(50)]
    public virtual string? EntityName { get; set; }

    /// <summary>
    /// 实体名称标准值
    /// </summary>
    [StringLength(50)]
    public virtual string? NormalizedEntityName { get; set; }

    /// <summary>
    /// 数据表名称
    /// </summary>
    [StringLength(50)]
    public virtual string? TableName { get; set; }

    /// <summary>
    /// 数据表名称标准值
    /// </summary>
    [StringLength(50)]
    public virtual string? NormalizedTableName { get; set; }

    /// <summary>
    /// 操作类型
    /// </summary>
    public virtual OperationLogType OperationLogType { get; set; }

    /// <summary>
    /// 操作用户主键
    /// </summary>
    [StringLength(36)]
    public virtual string? UserId { get; set; }

    /// <summary>
    /// 操作用户账号
    /// </summary>
    [StringLength(250)]
    public virtual string? UserName { get; set; }

    /// <summary>
    /// 操作用户姓名
    /// </summary>
    [StringLength(250)]
    public virtual string? RealName { get; set; }

    /// <summary>
    /// 操作客户端IP地址
    /// </summary>
    [StringLength(48)]
    public virtual string? OperateIP { get; set; }

    /// <summary>
    /// 操作时间
    /// </summary>
    public virtual DateTime? OperateDate { get; set; }

    public void Init<T>(T entity, OperationLogType logType, string? ipaddress, SignUser? user = null)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var type = entity.GetType();
        EntityName = type.Name;
        TableName = type.GetCustomAttribute<TableAttribute>(true)?.Name ?? type.Name;
        OperateIP = ipaddress;
        OperateDate = DateTime.Now;
        OperationLogType = logType;
        if (user != null)
        {
            UserId = user.Id;
            UserName = user.UserName;
            RealName = user.RealName;
        }
        NormalizedEntityName = EntityName.ToUpper();
        NormalizedTableName = TableName.ToUpper();
    }

    public override string ToString()
    {
        return EntityName ?? string.Empty;
    }
}
