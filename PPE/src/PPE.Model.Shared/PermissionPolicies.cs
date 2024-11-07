using Microsoft.AspNetCore.Authorization;

namespace PPE.Model.Shared
{
    /// <summary>
    /// 授权策略
    /// </summary>
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public PermissionRequirement(string name)
        {
            Name = name;
        }

        /// <summary>
        /// 授权策略名称
        /// </summary>
        public string Name { get; }
    }

    /// <summary>
    /// 授权策略名称
    /// </summary>
    public class PermissionPolicies
    {
        /// <summary>
        /// 管理员角色
        /// </summary>
        public const string AdministratorsRequirement = "Administrators";
        //public const string EngineersRequirement = "Engineers";
        /// <summary>
        /// 访问权限，系统菜单授权
        /// </summary>
        public const string AccessRequirement = "Access";
        /// <summary>
        /// 查询权限
        /// </summary>
        public const string FindRequirement = "Find";
        /// <summary>
        /// 创建权限，查询操作按钮授权
        /// </summary>
        public const string CreateRequirement = "Create";
        /// <summary>
        /// 导入权限，查询操作按钮授权
        /// </summary>
        public const string ImportRequirement = "Import";
        /// <summary>
        /// 导出权限，查询操作按钮授权
        /// </summary>
        public const string ExportRequirement = "Export";
        /// <summary>
        /// 编辑权限，查询操作按钮授权
        /// </summary>
        public const string EditRequirement = "Edit";
        /// <summary>
        /// 明细权限，查询操作按钮授权
        /// </summary>
        public const string DetailRequirement = "Detail";
        /// <summary>
        /// 删除权限，查询操作按钮授权
        /// </summary>
        public const string DeleteRequirement = "Delete";
        /// <summary>
        /// 恢复权限，查询操作按钮授权
        /// </summary>
        public const string RecoveryRequirement = "Recovery";
        /// <summary>
        /// 移除权限，查询操作按钮授权
        /// </summary>
        public const string RemoveRequirement = "Remove";
        /// <summary>
        /// 授权权限，查询操作按钮授权
        /// </summary>
        public const string GrantRequirement = "Grant";
    }
}