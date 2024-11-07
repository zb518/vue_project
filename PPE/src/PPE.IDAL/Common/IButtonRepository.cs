using PPE.DataModel;
using PPE.Model.Shared;

namespace PPE.IDAL;

/// <summary>
/// 操作按钮数据访问操作
/// </summary>
public interface IButtonRepository : IBaseRepository<Base_Button, CommonDbContext>
{
    /// <summary>
    /// 获取当前菜单下的最大排序
    /// </summary>
    /// <param name="menuId">系统菜单主键</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string?> FindMaxSortCodeAsync(string menuId, CancellationToken cancellationToken);

    /// <summary>
    /// 系统菜单主键查询系统菜单
    /// </summary>
    /// <param name="menuId">系统菜单主键</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Base_Menu?> FindMenuByIdAsync(string menuId, CancellationToken cancellationToken);

    /// <summary>
    /// 系统菜单名称查询系统菜单
    /// </summary>
    /// <param name="normalizedMenuName">系统菜单名称</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string?> FindMenuIdByNameAsync(string normalizedMenuName, CancellationToken cancellationToken);

    /// <summary>
    /// 分页查询
    /// </summary>
    /// <param name="parameter">分页查询参数 <see cref="DataTableParameter"/></param>
    /// <param name="menuId">系统菜单主键</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<DataTableResult<Base_Button>> FindPageAsync(DataTableParameter parameter, string menuId, CancellationToken cancellationToken);
}
