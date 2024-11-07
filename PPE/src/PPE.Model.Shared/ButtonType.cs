using System.ComponentModel.DataAnnotations;

namespace PPE.Model.Shared;
/// <summary>
/// 操作按钮类型
/// </summary>
public enum ButtonType
{
    /// <summary>
    /// 创建
    /// </summary>
    [Display(Name = "创建", GroupName = "Header")]
    Create,
    /// <summary>
    /// 导入
    /// </summary>
    [Display(Name = "导入", GroupName = "Header")]
    Import,
    /// <summary>
    /// 明细
    /// </summary>
    [Display(Name = "明细", GroupName = "Table")]
    Detail,
    /// <summary>
    /// 编辑
    /// </summary>
    [Display(Name = "编辑", GroupName = "Table")]
    Edit,
    /// <summary>
    /// 删除
    /// </summary>
    [Display(Name = "删除", GroupName = "Table")]
    Delete,
    /// <summary>
    /// 恢复
    /// </summary>
    [Display(Name = "恢复", GroupName = "Table")]
    Recovery,
    /// <summary>
    /// 移除
    /// </summary>
    [Display(Name = "移除", GroupName = "Table")]
    Remove,
    /// <summary>
    /// 导出
    /// </summary>
    [Display(Name = "导出", GroupName = "Header")]
    Export,
    /// <summary>
    /// 权限
    /// </summary>
    [Display(Name = "权限", GroupName = "Other")]
    Permit,
    /// <summary>
    /// 关系
    /// </summary>
    [Display(Name = "关系", GroupName = "Other")]
    Relate,
    /// <summary>
    /// 清空记录，一般用于日志
    /// </summary>
    [Display(Name = "清空", GroupName = "Other")]
    Clean,
    /// <summary>
    /// 其它
    /// </summary>
    [Display(Name = "其它", GroupName = "Other")]
    Other
}