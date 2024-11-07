using Microsoft.AspNetCore.Identity;
using PPE.Core;
using PPE.DataModel;
using PPE.Model.Shared;
using PPE.ModelDto;

namespace PPE.IDAL
{
    /// <summary>
    /// 角色数据访问操作接口
    /// </summary>
    public interface IRoleRepository : IRoleClaimStore<Base_Role>, IQueryableRoleStore<Base_Role>
    {
        IdentityFactory Identity { get; }
        /// <summary>
        /// 导出角色
        /// </summary>
        /// <param name="role"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IdentityResult> ImportAsync(Base_Role role, CancellationToken cancellationToken);

        /// <summary>
        /// 系统菜单是否授权给角色
        /// </summary>
        /// <param name="role">角色实体</param>
        /// <param name="menu">系统菜单实体</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> IsMenuInRoleAsync(Base_Role role, Base_Menu menu, CancellationToken cancellationToken);

        /// <summary>
        /// 添加系统菜单授权
        /// </summary>
        /// <param name="role">角色实体</param>
        /// <param name="menu">系统菜单实体</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task AddMenuToRoleAsync(Base_Role role, Base_Menu menu, CancellationToken cancellationToken);

        /// <summary>
        /// 移除系统菜单授权
        /// </summary>
        /// <param name="role">角色实体</param>
        /// <param name="menu">系统菜单实体</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task RemoveMenuFromRoleAsync(Base_Role role, Base_Menu menu, CancellationToken cancellationToken);

        /// <summary>
        /// 查询角色授权系统菜单
        /// </summary>
        /// <param name="role">角色实体</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IList<Base_Menu>?> FindMenusByRoleAsync(Base_Role role, CancellationToken cancellationToken);

        /// <summary>
        /// 操作按钮是否授权给角色
        /// </summary>
        /// <param name="role">角色实体</param>
        /// <param name="button">操作按钮实体</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> IsButtonInRoleAsync(Base_Role role, Base_Button button, CancellationToken cancellationToken);

        /// <summary>
        /// 添加操作按钮授权
        /// </summary>
        /// <param name="role">角色实体</param>
        /// <param name="button">操作按钮实体</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task AddButtonToRoleAsync(Base_Role role, Base_Button button, CancellationToken cancellationToken);

        /// <summary>
        /// 移除操作按钮授权
        /// </summary>
        /// <param name="role">角色实体</param>
        /// <param name="button">操作按钮实体</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task RemoveButtonFromRoleAsync(Base_Role role, Base_Button button, CancellationToken cancellationToken);

        /// <summary>
        /// 查询角色授权操作按钮
        /// </summary>
        /// <param name="role">角色实体</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IList<Base_Button>?> FindButtonsByRoleAsync(Base_Role role, CancellationToken cancellationToken);

        /// <summary>
        /// 查询指定系统菜单的角色授权操作按钮
        /// </summary>
        /// <param name="role">角色实体</param>
        /// <param name="menu">系统菜单</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IList<Base_Button>?> FindButtonsByRoleAsync(Base_Role role, Base_Menu menu, CancellationToken cancellationToken);

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="parameter">分页相关参数</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<DataTableResult<RoleListDto>> FindPageAsync(DataTableParameter parameter, CancellationToken cancellationToken);

        /// <summary>
        /// 恢复删除角色
        /// </summary>
        /// <param name="role">角色实体</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IdentityResult> RecoveryAsync(Base_Role role, CancellationToken cancellationToken);

        /// <summary>
        /// 移除角色，移除后角色不存在
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IdentityResult> RemoveAsync(Base_Role role, CancellationToken cancellationToken);
    }
}
