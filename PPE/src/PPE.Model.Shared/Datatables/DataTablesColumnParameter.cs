namespace PPE.Model.Shared;

/// <summary>
/// 
/// </summary>
public class DataTablesColumnParameter
{
    /// <summary>
    /// 
    /// </summary>
    /// <value></value>
    public string? name { get; set; }
    /// <summary>
    /// 
    /// </summary>
    /// <value></value>
    public string? data { get; set; }
    /// <summary>
	/// 标题
	/// </summary>
	/// <value></value>
	public string? title { get; set; }
    /// <summary>
    /// 列
    /// </summary>
    /// <value></value>
    public int target { get; set; }
    /// <summary>
    /// 是否支持搜索
    /// </summary>
    /// <value></value>
    public bool searchable { get; set; }
    /// <summary>
    /// 是否支持排序
    /// </summary>
    /// <value></value>
    public bool orderable { get; set; }
    /// <summary>
    /// 是否显示
    /// </summary>
    /// <value></value>
    public bool visible { get; set; }
    /// <summary>
    /// 数据类型
    /// </summary>
    /// <value></value>
    public string? DbType { get; set; }
}