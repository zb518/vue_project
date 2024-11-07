using PPE.DataModel;
using PPE.Model.Shared;
using PPE.ModelDto;

namespace PPE.IDAL;
/// <summary>
/// 系统菜单数据访问操作
/// </summary>
public interface IMenuRepository : IBaseRepository<Base_Menu, CommonDbContext>
{
    /// <summary>
    /// 设置系统菜单排序
    /// </summary>
    /// <param name="menu"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SetSortCodeAsync(Base_Menu menu, CancellationToken cancellationToken);

    /// <summary>
    /// 获取上级级数
    /// </summary>
    /// <param name="id">主键</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<int> GetLevelAsync(string id, CancellationToken cancellationToken);

    /// <summary>
    /// 分页查询系统菜单
    /// </summary>
    /// <param name="parameter">分页查询参数 <see cref="DataTableParameter"/></param>
    /// <param name="parentId">上级主键</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<DataTableResult<MenuDetailDto>> FindPageAsync(DataTableParameter parameter, string? parentId, CancellationToken cancellationToken);
}
