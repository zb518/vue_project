using System.ComponentModel.DataAnnotations;

namespace PPE.Model.Shared;
/// <summary>
/// 操作按钮分组
/// </summary>
public enum ButtonGroup
{
    /// <summary>
    /// 头部，位card-header
    /// </summary>
    [Display(Name = "头部")]
    Header,
    /// <summary>
    /// 数据表
    /// </summary>
    [Display(Name = "数据表")]
    Table,
    ///<summary>
    /// 右键
    /// </summary>
    [Display(Name = "右键")]
    Right,
    /// <summary>
    /// 其它
    /// </summary>
    [Display(Name = "其它")]
    Other,
}