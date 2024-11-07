using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PPE.Core;
using PPE.DataModel;
using PPE.IDAL;
using PPE.Model.Shared;
using PPE.ModelDto;
using System.Security.Claims;

namespace PPE.DAL.Common;

/// <summary>
/// 角色数据访问操作类
/// </summary>
public class RoleRepository : RoleStore<Base_Role, CommonDbContext, string, Base_UserRole, Base_RoleClaim>, IRoleRepository
{
    public RoleRepository(CommonDbContext context, IOperationLogRepository logRepository, IdentityFactory identityFactory, OperationErrorDescriber? describer = null) : base(context, describer)
    {
        LogStore = logRepository;
        ErrorDescriber = describer ?? new OperationErrorDescriber();
        Identity = identityFactory;
    }

    public IOperationLogRepository LogStore { get; }
    public new OperationErrorDescriber ErrorDescriber { get; set; }
    public IdentityFactory Identity { get; set; }

    private DbSet<Base_Role> RoleSet => Context.Roles;
    private DbSet<Base_UserRole> UserRoles => Context.UserRoles;
    private DbSet<Base_RoleClaim> RoleClaims => Context.RoleClaims;
    private DbSet<Base_Menu> Menus => Context.Menus;
    private DbSet<Base_Button> Buttons => Context.Buttons;
    private DbSet<Base_RoleMenu> RoleMenus => Context.RoleMenus;
    private DbSet<Base_RoleButton> RoleButtons => Context.RoleButtons;



    public override async Task<IdentityResult> CreateAsync(Base_Role role, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(role);
        role.SetCreate(Identity.GetSignUser());
        RoleSet.Add(role);
        await SaveChanges(cancellationToken);
        await LogStore.WriteForCreateAsync(role, OperationLogType.Create, cancellationToken);
        return IdentityResult.Success;
    }

    /// <summary>
    /// 导入角色
    /// </summary>
    /// <param name="role"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IdentityResult> ImportAsync(Base_Role role, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(role);
        role.SetCreate(Identity.GetSignUser());
        RoleSet.Add(role);
        await SaveChanges(cancellationToken);
        await LogStore.WriteForCreateAsync(role, OperationLogType.Import, cancellationToken);
        return IdentityResult.Success;
    }

    public override async Task<IdentityResult> UpdateAsync(Base_Role role, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(role);
        var owner = await FindByIdAsync(role.Id, cancellationToken);
        if (owner == null)
        {
            throw new InvalidOperationException("角色不存在");
        }
        if (!string.Equals(owner?.ConcurrencyStamp, role.ConcurrencyStamp))
        {
            return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
        }
        RoleSet.Attach(role);
        role.SetUpdate(Identity.GetSignUser());
        RoleSet.Update(role);
        try
        {
            await SaveChanges(cancellationToken);
            await LogStore.WriteForUpdateAsync(role, owner!, OperationLogType.Update, cancellationToken);
            return IdentityResult.Success;
        }
        catch (DbUpdateConcurrencyException)
        {
            return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
        }
    }


    /// <summary>
    /// 异步删除角色，只做删除标记，不移除记录
    /// </summary>
    /// <param name="role"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override async Task<IdentityResult> DeleteAsync(Base_Role role, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(role);
        var owner = await Roles.AsNoTracking().FirstOrDefaultAsync(r => r.Id == role.Id, cancellationToken);
        if (owner == null)
        {
            throw new InvalidOperationException($"Role id {role.Id} does not found.");
        }
        if (!string.Equals(owner.ConcurrencyStamp, role.ConcurrencyStamp))
        {
            return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
        }
        RoleSet.Attach(role);
        role.SetUpdate(Identity.GetSignUser());
        RoleSet.Update(role);
        try
        {
            await SaveChanges(cancellationToken);
            await LogStore.WriteForUpdateAsync(role, owner, OperationLogType.Delete, cancellationToken);
            return IdentityResult.Success;
        }
        catch (DbUpdateConcurrencyException)
        {
            return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
        }
    }

    public override Task AddClaimAsync(Base_Role role, Claim claim, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(role);
        ArgumentNullException.ThrowIfNull(claim);
        var roleClaim = CreateRoleClaim(role, claim);
        RoleClaims.Add(roleClaim);
        LogStore.WriteForCreateAsync(roleClaim, OperationLogType.Create, cancellationToken);
        return Task.FromResult(false);
    }


    public override async Task RemoveClaimAsync(Base_Role role, Claim claim, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(role);
        ArgumentNullException.ThrowIfNull(claim);
        var claims = await RoleClaims.Where(rc => rc.RoleId.Equals(role.Id) && rc.ClaimValue == claim.Value && rc.ClaimType == claim.Type).ToListAsync(cancellationToken);
        foreach (var c in claims)
        {
            RoleClaims.Remove(c);
            await LogStore.WriteForRemoveAsync(c, cancellationToken);
        }
    }

    /// <summary>
    /// 移除角色中的所有声明
    /// </summary>
    /// <param name="role"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task RemoveAllRoleClaimsAsync(Base_Role role, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(role);
        var roleClaims = await RoleClaims.Where(r => r.RoleId.Equals(role.Id)).ToListAsync(cancellationToken);
        if (roleClaims?.Count > 0)
        {
            foreach (var c in roleClaims)
            {
                RoleClaims.Remove(c);
                await LogStore.WriteForRemoveAsync(c, cancellationToken);
            }
        }
    }

    /// <summary>
    /// 系统菜单是否授权给角色
    /// </summary>
    /// <param name="role">角色实体</param>
    /// <param name="menu">系统菜单实体</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> IsMenuInRoleAsync(Base_Role role, Base_Menu menu, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(role);
        ArgumentNullException.ThrowIfNull(menu);
        return await RoleMenus.AnyAsync(rm => rm.RoleId.Equals(role.Id) && rm.MenuId.Equals(menu.Id), cancellationToken);
    }

    /// <summary>
    /// 添加系统菜单授权
    /// </summary>
    /// <param name="role">角色实体</param>
    /// <param name="menu">系统菜单实体</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task AddMenuToRoleAsync(Base_Role role, Base_Menu menu, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(role);
        ArgumentNullException.ThrowIfNull(menu);
        var roleMenu = new Base_RoleMenu
        {
            RoleId = role.Id,
            MenuId = menu.Id,
        };
        roleMenu.SetCreate(Identity.GetSignUser());
        RoleMenus.Add(roleMenu);
        await SaveChanges(cancellationToken);
        await LogStore.WriteForCreateAsync(roleMenu, OperationLogType.Create, cancellationToken);
    }

    /// <summary>
    /// 移除系统菜单授权
    /// </summary>
    /// <param name="role">角色实体</param>
    /// <param name="menu">系统菜单实体</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task RemoveMenuFromRoleAsync(Base_Role role, Base_Menu menu, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(role);
        ArgumentNullException.ThrowIfNull(menu);
        var roleMenu = await RoleMenus.FindAsync([role.Id, menu.Id], cancellationToken);
        if (roleMenu != null)
        {
            RoleMenus.Remove(roleMenu);
            await SaveChanges(cancellationToken);
            await LogStore.WriteForRemoveAsync(roleMenu, cancellationToken);
        }
    }

    /// <summary>
    /// 查询角色授权系统菜单
    /// </summary>
    /// <param name="role">角色实体</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IList<Base_Menu>?> FindMenusByRoleAsync(Base_Role role, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(role);
        var query = from menu in Menus
                    join rm in RoleMenus on menu.Id equals rm.MenuId
                    where rm.RoleId.Equals(role.Id)
                    orderby menu.SortCode
                    select menu;
        return await query.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// 操作按钮是否授权给角色
    /// </summary>
    /// <param name="role">角色实体</param>
    /// <param name="button">操作按钮实体</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> IsButtonInRoleAsync(Base_Role role, Base_Button button, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(role);
        ArgumentNullException.ThrowIfNull(button);
        return await RoleButtons.AnyAsync(rb => rb.RoleId.Equals(role.Id) && rb.ButtonId.Equals(button.Id), cancellationToken);
    }

    /// <summary>
    /// 添加操作按钮授权
    /// </summary>
    /// <param name="role">角色实体</param>
    /// <param name="button">操作按钮实体</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task AddButtonToRoleAsync(Base_Role role, Base_Button button, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(role);
        ArgumentNullException.ThrowIfNull(button);
        var roleButton = new Base_RoleButton
        {
            RoleId = role.Id,
            ButtonId = button.Id
        };
        roleButton.SetCreate(Identity.GetSignUser());
        RoleButtons.Add(roleButton);
        await SaveChanges(cancellationToken);
        await LogStore.WriteForCreateAsync(roleButton, OperationLogType.Create, cancellationToken);
    }

    /// <summary>
    /// 移除操作按钮授权
    /// </summary>
    /// <param name="role">角色实体</param>
    /// <param name="button">操作按钮实体</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task RemoveButtonFromRoleAsync(Base_Role role, Base_Button button, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(role);
        ArgumentNullException.ThrowIfNull(button);
        var roleButton = await RoleButtons.FindAsync([role.Id, button.Id], cancellationToken);
        if (roleButton != null)
        {
            RoleButtons.Remove(roleButton);
            await SaveChanges(cancellationToken);
            await LogStore.WriteForRemoveAsync(roleButton, cancellationToken);
        }
    }


    /// <summary>
    /// 查询角色授权操作按钮
    /// </summary>
    /// <param name="role">角色实体</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IList<Base_Button>?> FindButtonsByRoleAsync(Base_Role role, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(role);
        var query = from button in Buttons
                    join rb in RoleButtons on button.Id equals rb.ButtonId
                    where rb.RoleId.Equals(role.Id)
                    orderby button.SortCode
                    select button;
        return await query.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// 查询指定系统菜单的角色授权操作按钮
    /// </summary>
    /// <param name="role">角色实体</param>
    /// <param name="menu">系统菜单</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IList<Base_Button>?> FindButtonsByRoleAsync(Base_Role role, Base_Menu menu, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(role);
        ArgumentNullException.ThrowIfNull(menu);
        var query = from button in Buttons
                    join rb in RoleButtons on button.Id equals rb.ButtonId
                    where rb.RoleId.Equals(role.Id) && button.MenuId.Equals(menu.Id)
                    orderby button.SortCode
                    select button;
        return await query.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// 分页查询
    /// </summary>
    /// <param name="parameter">分页相关参数</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<DataTableResult<RoleListDto>> FindPageAsync(DataTableParameter parameter, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(parameter);
        var query = Roles.AsNoTracking()
        .Select(r => new RoleListDto
        {
            Id = r.Id,
            Name = r.Name,
            NormalizedName = r.NormalizedName,
            IsDeleted = r.IsDeleted,
            Description = r.Description
        });
        return ExpressionExtensions.FindPageAsync(query, parameter, cancellationToken: cancellationToken);
    }



    /// <summary>
    /// 恢复删除角色
    /// </summary>
    /// <param name="role">角色实体</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IdentityResult> RecoveryAsync(Base_Role role, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(role);
        var owner = await Roles.AsNoTracking().FirstOrDefaultAsync(r => r.Id == role.Id, cancellationToken);
        if (owner == null)
        {
            throw new InvalidOperationException($"Role id {role.Id} does not found.");
        }
        if (!string.Equals(owner.ConcurrencyStamp, role.ConcurrencyStamp))
        {
            return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
        }
        RoleSet.Attach(role);
        role.SetUpdate(Identity.GetSignUser());
        RoleSet.Update(role);
        try
        {
            await SaveChanges(cancellationToken);
            await LogStore.WriteForUpdateAsync(role, owner, OperationLogType.Recovery, cancellationToken);
            return IdentityResult.Success;
        }
        catch (DbUpdateConcurrencyException)
        {
            return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
        }
    }

    /// <summary>
    /// 移除角色，移除后角色不存在
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IdentityResult> RemoveAsync(Base_Role role, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(role);
        RoleSet.Remove(role);
        try
        {
            await SaveChanges(cancellationToken);
            await LogStore.WriteForRemoveAsync(role, cancellationToken);
            return IdentityResult.Success;
        }
        catch (DbUpdateConcurrencyException)
        {
            return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
        }
    }
}