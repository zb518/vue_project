using Microsoft.EntityFrameworkCore;
using PPE.Core;
using PPE.DataModel;
using PPE.IDAL;
using PPE.Model.Shared;
using PPE.ModelDto;

namespace PPE.DAL;
/// <summary>
/// 系统菜单数据访问操作
/// </summary>
public class MenuRepository : BaseRepository<Base_Menu, CommonDbContext>, IMenuRepository
{
    public MenuRepository(CommonDbContext context, OperationErrorDescriber describer, IdentityFactory identityFactory, IOperationLogRepository logRepository) : base(context, describer, identityFactory, logRepository)
    {
    }
    private DbSet<Base_Button> Buttons => Context.Buttons;
    private DbSet<Base_Role> Roles => Context.Roles;
    private DbSet<Base_User> Users => Context.Users;
    private DbSet<Base_UserMenu> UserMenus => Context.UserMenus;
    private DbSet<Base_RoleMenu> RoleMenus => Context.RoleMenus;

    /// <summary>
    /// 设置系统菜单排序
    /// </summary>
    /// <param name="menu"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task SetSortCodeAsync(Base_Menu menu, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(menu);
        var maxSortCode = await Entities.Where(m => m.ParentId == menu.ParentId).OrderBy(m => m.SortCode).MaxAsync(m => m.SortCode, cancellationToken);
        if (maxSortCode == null)
        {
            if (menu.ParentId == Guid.Empty.ToString())
            {
                menu.SortCode = "1";
            }
            else
            {
                var sortCode = await Entities.Where(m => m.Id == menu.ParentId).Select(m => m.SortCode).FirstOrDefaultAsync(cancellationToken);
                menu.SortCode = sortCode + "01";
            }
        }
        else
        {
            menu.SortCode = string.Format("{0}", Convert.ToInt64(maxSortCode) + 1);
        }
    }

    /// <summary>
    /// 获取上级级数
    /// </summary>
    /// <param name="id">主键</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<int> GetLevelAsync(string id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNullOrWhiteSpace(id);
        return Entities.Where(m => m.Id == id).Select(m => m.Level).FirstOrDefaultAsync(cancellationToken);
    }



    /// <summary>
    /// 分页查询系统菜单
    /// </summary>
    /// <param name="parameter">分页查询参数 <see cref="DataTableParameter"/></param>
    /// <param name="parentId">上级主键</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<DataTableResult<MenuDetailDto>> FindPageAsync(DataTableParameter parameter, string? parentId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(parameter);
        var query = from menu in Entities
                    select new MenuDetailDto
                    {
                        Id = menu.Id,
                        ParentId = menu.ParentId,
                        Name = menu.Name,
                        NormalizedName = menu.NormalizedName,
                        Title = menu.Title,
                        Icon = menu.Icon,
                        Area = menu.Area,
                        NormalizedArea = menu.NormalizedArea,
                        Page = menu.Page,
                        NormalizedPage = menu.NormalizedPage,
                        Level = menu.Level,
                        IsDeleted = menu.IsDeleted,
                        Description = menu.Description,
                        CreateUserName = menu.CreateUserName,
                        CreateRealName = menu.CreateRealName,
                        CreateDate = menu.CreateDate,
                        UpdateUserName = menu.UpdateUserName,
                        UpdateRealName = menu.UpdateRealName,
                        UpdateDate = menu.UpdateDate,
                    };
        if (parentId != null)
        {
            query = query.Where(m => m.Id == parentId || m.ParentId == parentId);
        }
        return ExpressionExtensions.FindPageAsync(query, parameter);
    }
}
