using Microsoft.EntityFrameworkCore;
using PPE.Core;
using PPE.DataModel;
using PPE.IDAL;
using PPE.Model.Shared;

namespace PPE.DAL;

/// <summary>
/// 操作按钮数据访问操作
/// </summary>
public class ButtonRepository : BaseRepository<Base_Button, CommonDbContext>, IButtonRepository
{
    public ButtonRepository(CommonDbContext context, OperationErrorDescriber describer, IdentityFactory identityFactory, IOperationLogRepository logRepository) : base(context, describer, identityFactory, logRepository)
    {
    }

    private DbSet<Base_Menu> Menus => Context.Menus;
    private DbSet<Base_Button> Buttons => Context.Buttons;
    private DbSet<Base_User> Users => Context.Users;
    private DbSet<Base_Role> Roles => Context.Roles;
    private DbSet<Base_RoleMenu> RoleMenus => Context.RoleMenus;
    private DbSet<Base_UserMenu> UserMenus => Context.UserMenus;

    /// <summary>
    /// 获取当前菜单下的最大排序
    /// </summary>
    /// <param name="menuId">系统菜单主键</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<string?> FindMaxSortCodeAsync(string menuId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentException.ThrowIfNullOrWhiteSpace(menuId);
        return Buttons.Where(b => b.MenuId == menuId).MaxAsync(b => b.SortCode, cancellationToken);
    }

    /// <summary>
    /// 系统菜单主键查询系统菜单
    /// </summary>
    /// <param name="menuId">系统菜单主键</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Base_Menu?> FindMenuByIdAsync(string menuId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentException.ThrowIfNullOrWhiteSpace(menuId);
        return Menus.AsNoTracking().SingleOrDefaultAsync(m => m.Id == menuId, cancellationToken);
    }

    /// <summary>
    /// 系统菜单名称查询系统菜单
    /// </summary>
    /// <param name="normalizedMenuName">系统菜单名称</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<string?> FindMenuIdByNameAsync(string normalizedMenuName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentException.ThrowIfNullOrWhiteSpace(normalizedMenuName);
        return Menus.AsNoTracking().Where(m => m.NormalizedName == normalizedMenuName).Select(m => m.Id).FirstOrDefaultAsync(cancellationToken);
    }



    /// <summary>
    /// 分页查询
    /// </summary>
    /// <param name="parameter">分页查询参数 <see cref="DataTableParameter"/></param>
    /// <param name="menuId">系统菜单主键</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<DataTableResult<Base_Button>> FindPageAsync(DataTableParameter parameter, string menuId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(parameter);
        ArgumentException.ThrowIfNullOrWhiteSpace(menuId);
        var query = Entities.Where(b => b.MenuId == menuId);
        return ExpressionExtensions.FindPageAsync(query, parameter, cancellationToken: cancellationToken);
    }
}