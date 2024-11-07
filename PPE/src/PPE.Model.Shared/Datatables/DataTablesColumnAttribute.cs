namespace PPE.Model.Shared;

/// <summary>
/// DataTables 属性
/// </summary>
public class DataTablesColumnAttribute : Attribute
{
    public DataTablesColumnAttribute()
    {
    }

    public DataTablesColumnAttribute(bool searchable, bool orderable, bool visible)
    {
        Searchable = searchable;
        Orderable = orderable;
        Visible = visible;
    }
    /// <summary>
    /// 是否支持搜索
    /// </summary>
    /// <value></value>
    public bool Searchable { get; set; }
    /// <summary>
    /// 是否支持排序
    /// </summary>
    /// <value></value>
    public bool Orderable { get; set; }
    /// <summary>
    /// 是否显示
    /// </summary>
    /// <value></value>
    public bool Visible { get; set; }
}