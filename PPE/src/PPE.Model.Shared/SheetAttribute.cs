namespace PPE.Model.Shared;
/// <summary>
/// Excel表格属性
/// </summary>
public class SheetAttribute : Attribute
{
    public SheetAttribute(string name)
    {
        Name = name;
    }
    /// <summary>
    /// 表名称
    /// </summary>
    /// <value></value>
    public string Name { get; set; }
}