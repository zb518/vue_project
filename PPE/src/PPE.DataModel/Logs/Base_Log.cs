using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPE.DataModel;

/// <summary>
/// log4net 日志
/// </summary>
[Table("Base_Log")]
public class Base_Log
{
    /// <summary>
    /// 主键
    /// </summary>
    /// <value></value>
    [Key]
    public virtual ulong Id { get; set; }

    /// <summary>
    /// 日期
    /// </summary>
    /// <value></value>
    public virtual DateTime Date { get; set; }

    /// <summary>
    /// 线程
    /// </summary>
    /// <value></value>
    [StringLength(255)]
    public virtual string? Thread { get; set; }

    /// <summary>
    /// 级别
    /// </summary>
    /// <value></value>
    [StringLength(50)]
    public virtual string? Level { get; set; }

    /// <summary>
    /// 日志
    /// </summary>
    /// <value></value>
    [StringLength(255)]
    public virtual string? Logger { get; set; }

    /// <summary>
    /// 消息
    /// </summary>
    /// <value></value>
    [StringLength(4000)]
    public virtual string? Message { get; set; }

    /// <summary>
    /// 异常
    /// </summary>
    /// <value></value>
    [StringLength(2000)]
    public virtual string? Exception { get; set; }

    public override string ToString()
    {
        return Message ?? string.Empty;
    }
}
