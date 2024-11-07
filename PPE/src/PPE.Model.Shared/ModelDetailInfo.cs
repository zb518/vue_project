using System.Data;

namespace PPE.Model.Shared;
/// <summary>
/// 模型详细信息
/// </summary>
public class ModelDetailInfo
{
    /// <summary>
    /// 属性或字段名称
    /// </summary>
    /// <value></value>
    public string? Name { get; set; }

    /// <summary>
    /// 显示名称 <see cref="DisplayAttribute"/>
    /// </summary>
    /// <value></value>
    public string? DisplayName { get; set; }

    /// <summary>
    /// 分组名称 <see cref="DisplayAttribute"/>
    /// </summary>
    /// <value></value>
    public string? GroupName { get; set; }

    /// <summary>
    /// 排序 <see cref="DisplayAttribute"/>
    /// </summary>
    /// <value></value>
    public int Order { get; set; }

    /// <summary>
    /// 是否为主键 <see cref="KeyAttribute"/>
    /// </summary>
    /// <value></value>
    public bool IsKey { get; set; }

    /// <summary>
    /// 自定义列名称，<see cref="ColumnAttribute"/>
    /// </summary>
    /// <value></value>
    public string? ColumnName { get; set; }

    /// <summary>
    /// 数据类型 <see cref="DbType"/>
    /// </summary>
    /// <value></value>
    public DbType DbType { get; set; }

    /// <summary>
    /// 属性或字段值
    /// </summary>
    /// <value></value>
    public object? Value { get; set; }
}