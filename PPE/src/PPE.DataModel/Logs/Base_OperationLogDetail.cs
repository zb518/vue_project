using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace PPE.DataModel;

/// <summary>
/// 操作日志明细
/// </summary>
[Table("Base_OperationLogDetail")]
public class Base_OperationLogDetail
{
    public Base_OperationLogDetail()
    {
    }

    public Base_OperationLogDetail(ulong operateLogId, string? fieldName, string? columnName, DbType dbType, string? oldValue, string? newValue)
    {
        OperateLogId = operateLogId;
        FieldName = fieldName;
        ColumnName = columnName;
        DbType = dbType;
        OldValue = oldValue;
        NewValue = newValue;
    }

    /// <summary>
    /// 主键
    /// </summary>
    [Key]
    public virtual ulong Id { get; set; }

    /// <summary>
    /// 主日志主键
    /// </summary>
    [ForeignKey(nameof(Base_OperationLog))]
    public virtual ulong OperateLogId { get; set; }

    /// <summary>
    /// 字段名称
    /// </summary>
    [StringLength(50)]
    public virtual string? FieldName { get; set; }

    /// <summary>
    /// 字段名称标准值
    /// </summary>
    [StringLength(50)]
    public virtual string? NormalizedFieldName { get; set; }

    /// <summary>
    /// 列名称
    /// </summary>
    [StringLength(50)]
    public virtual string? ColumnName { get; set; }

    /// <summary>
    /// 列名称标准值
    /// </summary>
    [StringLength(50)]
    public virtual string? NormalizedColumnName { get; set; }

    /// <summary>
    /// 数据类型
    /// </summary>
    public virtual DbType DbType { get; set; }

    /// <summary>
    /// 旧值
    /// </summary>
    public virtual string? OldValue { get; set; }

    /// <summary>
    /// 新值
    /// </summary>
    public virtual string? NewValue { get; set; }



    public override string ToString()
    {
        return FieldName ?? string.Empty;
    }
}
