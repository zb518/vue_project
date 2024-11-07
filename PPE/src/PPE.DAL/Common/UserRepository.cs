using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PPE.Core;
using PPE.DataModel;
using PPE.IDAL;
using PPE.Model.Shared;
using PPE.ModelDto;
using System.Globalization;
using System.Security.Claims;

namespace PPE.DAL.Common;
/// <summary>
/// 用户数据访问操作
/// </summary>
public class UserRepository : UserStore<Base_User, Base_Role, CommonDbContext, string, Base_UserClaim, Base_UserRole, Base_UserLogin, Base_UserToken, Base_RoleClaim>, IUserRepository
{
    public UserRepository(CommonDbContext context, IOperationLogRepository logRepository, IdentityFactory identityFactory, IServiceProvider service, OperationErrorDescriber? describer = null) : base(context, describer)
    {
        LogStore = logRepository;
        Service = service;
        ErrorDescriber = describer ?? new OperationErrorDescriber();
        Identity = identityFactory;
    }

    public new OperationErrorDescriber ErrorDescriber { get; set; }
    public IServiceProvider Service { get; }
    public IOperationLogRepository LogStore { get; }
    public IdentityFactory Identity { get; set; }


    private DbSet<Base_Menu> Menus => Context.Menus;
    private DbSet<Base_Button> Buttons => Context.Buttons;
    private DbSet<Base_UserMenu> UserMenus => Context.UserMenus;
    private DbSet<Base_UserButton> UserButtons => Context.UserButtons;
    private DbSet<Base_UserRole> UserRoles => Context.UserRoles;
    private DbSet<Base_Role> Roles => Context.Roles;
    private DbSet<Base_UserLogin> UserLogins => Context.UserLogins;
    private DbSet<Base_UserToken> UserTokens => Context.UserTokens;
    private DbSet<Base_UserClaim> UserClaims => Context.UserClaims;


    private IMenuRepository GetMenuStore()
    {
        return Service.GetRequiredService<IMenuRepository>() ?? throw new NotSupportedException();
    }

    /// <summary>
    /// 导入用户
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual async Task<IdentityResult> ImportAsync(Base_User user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        user.SetCreate(Identity.GetSignUser());
        Context.Add(user);
        await SaveChanges(cancellationToken);
        await LogStore.WriteForCreateAsync(user, OperationLogType.Import, cancellationToken);
        return IdentityResult.Success;
    }

    /// <summary>
    /// 创建用户
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override async Task<IdentityResult> CreateAsync(Base_User user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        user.SetCreate(Identity.GetSignUser());
        Context.Add(user);
        await SaveChanges(cancellationToken);
        await LogStore.WriteForCreateAsync(user, OperationLogType.Create, cancellationToken);
        return IdentityResult.Success;
    }

    /// <summary>
    /// 更新用户
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override async Task<IdentityResult> UpdateAsync(Base_User user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        var owner = await Users.AsNoTracking().SingleAsync(u => u.Id.Equals(user.Id), cancellationToken);
        if (owner == null)
        {
            throw new InvalidOperationException($"用户主键 {user.Id} 不正确");
        }
        if (owner.NormalizedUserName == "ADMIN" && user.NormalizedUserName != "ADMIN")
        {
            return IdentityResult.Failed(ErrorDescriber.AdministratorCannotModifyUserName(owner.UserName));
        }
        if (!string.Equals(user.ConcurrencyStamp, owner.ConcurrencyStamp))
        {
            return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
        }
        Context.Attach(user);
        user.SetUpdate(Identity.GetSignUser());
        Context.Update(user);
        try
        {
            await SaveChanges(cancellationToken);
            await LogStore.WriteForUpdateAsync(user, owner, OperationLogType.Update, cancellationToken);
            return IdentityResult.Success;
        }
        catch (DbUpdateConcurrencyException)
        {
            return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
        }
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override async Task<IdentityResult> DeleteAsync(Base_User user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        if (user.NormalizedUserName == "ADMINS")
        {
            return IdentityResult.Failed(ErrorDescriber.AdministratorCannotDelete(user.UserName, user.RealName));
        }
        Context.Remove(user);
        try
        {
            await SaveChanges(cancellationToken);
            await LogStore.WriteForRemoveAsync(user, cancellationToken);
            return IdentityResult.Success;
        }
        catch (DbUpdateConcurrencyException)
        {
            return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
        }
    }

    public override async Task AddToRoleAsync(Base_User user, string normalizedRoleName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        if (string.IsNullOrWhiteSpace(normalizedRoleName))
        {
            throw new ArgumentException(Resources.ValueCannotBeNullOrEmpty, nameof(normalizedRoleName));
        }
        var roleEntity = await FindRoleAsync(normalizedRoleName, cancellationToken);
        if (roleEntity == null)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.RoleNotFound, normalizedRoleName));
        }
        var userRole = CreateUserRole(user, roleEntity);
        userRole.SetCreate(Identity.GetSignUser());
        Context.Add(userRole);
        await SaveChanges(cancellationToken);
        await LogStore.WriteForCreateAsync(userRole, OperationLogType.Create, cancellationToken);
    }

    public override async Task RemoveFromRoleAsync(Base_User user, string normalizedRoleName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        if (string.IsNullOrWhiteSpace(normalizedRoleName))
        {
            throw new ArgumentException(Resources.ValueCannotBeNullOrEmpty, nameof(normalizedRoleName));
        }
        var roleEntity = await FindRoleAsync(normalizedRoleName, cancellationToken);
        if (roleEntity != null)
        {
            var userRole = await FindUserRoleAsync(user.Id, roleEntity.Id, cancellationToken);
            if (userRole != null)
            {
                Context.Remove(userRole);
                await SaveChanges(cancellationToken);
                await LogStore.WriteForRemoveAsync(userRole, cancellationToken);
            }
        }
    }

    public override async Task AddClaimsAsync(Base_User user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(claims);
        foreach (var claim in claims)
        {
            var userClaim = CreateUserClaim(user, claim);
            Context.Add(userClaim);
            await SaveChanges(cancellationToken);
            await LogStore.WriteForCreateAsync(userClaim, OperationLogType.Create, cancellationToken);
        }
    }

    public override async Task ReplaceClaimAsync(Base_User user, Claim claim, Claim newClaim, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(claim);
        ArgumentNullException.ThrowIfNull(newClaim);

        var matchedClaims = await Context.UserClaims.Where(uc => uc.UserId.Equals(user.Id) && uc.ClaimValue == claim.Value && uc.ClaimType == claim.Type).ToListAsync(cancellationToken);
        foreach (var matchedClaim in matchedClaims)
        {
            var userClaim = matchedClaim;
            matchedClaim.ClaimValue = newClaim.Value;
            matchedClaim.ClaimType = newClaim.Type;
            await SaveChanges(cancellationToken);
            await LogStore.WriteForUpdateAsync(matchedClaim, userClaim, OperationLogType.Update, cancellationToken);
        }
    }

    public override async Task RemoveClaimsAsync(Base_User user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(claims);
        foreach (var claim in claims)
        {
            var matchedClaims = await Context.UserClaims.Where(uc => uc.UserId.Equals(user.Id) && uc.ClaimValue == claim.Value && uc.ClaimType == claim.Type).ToListAsync(cancellationToken);
            foreach (var c in matchedClaims)
            {
                Context.UserClaims.Remove(c);
                await SaveChanges(cancellationToken);
                await LogStore.WriteForRemoveAsync(c, cancellationToken);
            }
        }
    }

    public override async Task AddLoginAsync(Base_User user, UserLoginInfo login, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(login);
        var userLogin = CreateUserLogin(user, login);
        Context.UserLogins.Add(userLogin);
        await SaveChanges(cancellationToken);
        await LogStore.WriteForCreateAsync(userLogin, OperationLogType.Create, cancellationToken);
    }

    public override async Task RemoveLoginAsync(Base_User user, string loginProvider, string providerKey, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        var entry = await FindUserLoginAsync(user.Id, loginProvider, providerKey, cancellationToken);
        if (entry != null)
        {
            Context.UserLogins.Remove(entry);
            await SaveChanges(cancellationToken);
            await LogStore.WriteForRemoveAsync(entry, cancellationToken);
        }
    }

    protected override async Task AddUserTokenAsync(Base_UserToken token)
    {
        Context.UserTokens.Add(token);
        await SaveChanges(CancellationToken.None);
        await LogStore.WriteForCreateAsync(token, OperationLogType.Create, CancellationToken.None);
    }

    protected override async Task RemoveUserTokenAsync(Base_UserToken token)
    {
        Context.UserTokens.Add(token);
        await SaveChanges(CancellationToken.None);
        await LogStore.WriteForRemoveAsync(token, CancellationToken.None);
    }


    private async Task RemoveAllUserClaimsAsync(Base_User user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        var userClaims = await Context.UserClaims.Where(u => u.UserId.Equals(user.Id)).ToListAsync(cancellationToken);
        if (userClaims.Any())
        {
            foreach (var claim in userClaims)
            {
                Context.UserClaims.Remove(claim);
                await SaveChanges(cancellationToken);
                await LogStore.WriteForRemoveAsync(claim, cancellationToken);
            }
        }
    }

    private async Task RemoveAllUserRolesAsync(Base_User user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        var userRoles = await Context.UserRoles.Where(u => u.UserId.Equals(user.Id)).ToListAsync(cancellationToken);
        if (userRoles.Any())
        {
            foreach (var ur in userRoles)
            {
                Context.UserRoles.Remove(ur);
                await SaveChanges(cancellationToken);
                await LogStore.WriteForRemoveAsync(ur, cancellationToken);
            }
        }
    }

    private async Task RemoveAllUserLoginsAsync(Base_User user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        var userLogins = await Context.UserLogins.Where(u => u.UserId.Equals(user.Id)).ToListAsync(cancellationToken);
        if (userLogins.Any())
        {
            foreach (var item in userLogins)
            {
                Context.UserLogins.Remove(item);
                await SaveChanges(cancellationToken);
                await LogStore.WriteForRemoveAsync(item, cancellationToken);
            }
        }
    }

    private async Task RemoveAllUserTokensAsync(Base_User user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        var userTokens = await Context.UserTokens.Where(u => u.UserId.Equals(user.Id)).ToListAsync(cancellationToken);
        if (userTokens.Any())
        {
            foreach (var item in userTokens)
            {
                Context.UserTokens.Remove(item);
                await SaveChanges(cancellationToken);
                await LogStore.WriteForRemoveAsync(item, cancellationToken);
            }
        }
    }


    /// <summary>
    /// 系统菜单是否授权给用户
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <param name="menu">系统菜单实体</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> IsMenuInUserAsync(Base_User user, Base_Menu menu, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(menu);
        return await UserMenus.AnyAsync(rm => rm.UserId.Equals(user.Id) && rm.MenuId.Equals(menu.Id), cancellationToken);
    }

    /// <summary>
    /// 添加系统菜单授权
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <param name="menu">系统菜单实体</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task AddMenuToUserasync(Base_User user, Base_Menu menu, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(menu);
        var userMenu = new Base_UserMenu
        {
            UserId = user.Id,
            MenuId = menu.Id,
        };
        userMenu.SetCreate(Identity.GetSignUser());
        UserMenus.Add(userMenu);
        await SaveChanges(cancellationToken);
        await LogStore.WriteForCreateAsync(userMenu, OperationLogType.Create, cancellationToken);
    }

    /// <summary>
    /// 移除系统菜单授权
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <param name="menu">系统菜单实体</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task RemoveMenuFromUserasync(Base_User user, Base_Menu menu, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(menu);
        var userMenu = await UserMenus.FindAsync([user.Id, menu.Id], cancellationToken);
        if (userMenu != null)
        {
            UserMenus.Remove(userMenu);
            await SaveChanges(cancellationToken);
            await LogStore.WriteForRemoveAsync(userMenu, cancellationToken);
        }
    }

    /// <summary>
    /// 查询用户授权系统菜单
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IList<Base_Menu>?> FindMenusByUserAsync(Base_User user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        var query = from menu in Menus
                    join um in UserMenus on menu.Id equals um.MenuId
                    where um.UserId.Equals(user.Id) && !menu.IsDeleted
                    orderby menu.SortCode
                    select menu;
        return await query.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// 操作按钮是否授权给用户
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <param name="button">操作按钮实体</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> IsButtonInUserAsync(Base_User user, Base_Button button, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(button);
        return await UserButtons.AnyAsync(rb => rb.UserId.Equals(user.Id) && rb.ButtonId.Equals(button.Id), cancellationToken);
    }

    /// <summary>
    /// 添加操作按钮授权
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <param name="button">操作按钮实体</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task AddButtonToUserAsync(Base_User user, Base_Button button, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(button);
        var userButton = new Base_UserButton
        {
            UserId = user.Id,
            ButtonId = button.Id
        };
        userButton.SetCreate(Identity.GetSignUser());
        UserButtons.Add(userButton);
        await SaveChanges(cancellationToken);
        await LogStore.WriteForCreateAsync(userButton, OperationLogType.Create, cancellationToken);
    }

    /// <summary>
    /// 移除操作按钮授权
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <param name="button">操作按钮实体</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task RemoveButtonFromUserAsync(Base_User user, Base_Button button, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(button);
        var userButton = await UserButtons.FindAsync([user.Id, button.Id], cancellationToken);
        if (userButton != null)
        {
            UserButtons.Remove(userButton);
            await SaveChanges(cancellationToken);
            await LogStore.WriteForRemoveAsync(userButton, cancellationToken);
        }
    }

    /// <summary>
    /// 查询用户授权操作按钮
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IList<Base_Button>?> FindButtonsByUserAsync(Base_User user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        var query = from button in Buttons
                    join rb in UserButtons on button.Id equals rb.ButtonId
                    where rb.UserId.Equals(user.Id)
                    orderby button.SortCode
                    select button;
        return await query.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// 查询指定系统菜单的用户授权操作按钮
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <param name="menu">系统菜单</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IList<Base_Button>?> FindButtonsByUserAsync(Base_User user, Base_Menu menu, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(menu);
        var query = from button in Buttons
                    join rb in UserButtons on button.Id equals rb.ButtonId
                    where rb.UserId.Equals(user.Id) && button.MenuId.Equals(menu.Id)
                    orderby button.SortCode
                    select button;
        return await query.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// 更新用户登录信息
    /// </summary>
    /// <param name="user">登录用户</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IdentityResult> UpdateLoginInfoAsync(Base_User user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        var owner = await Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id.Equals(user.Id), cancellationToken);
        Context.Attach(user);
        Context.Update(user);
        await SaveChanges(cancellationToken);
        await LogStore.WriteForUpdateAsync(user, owner!, OperationLogType.Update, cancellationToken);
        return IdentityResult.Success;
    }

    /// <summary>
    /// 查询单个用户授权系统菜单
    /// </summary>
    /// <param name="user">用户实例</param>
    /// <param name="menu">系统菜单实例</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Base_UserMenu?> FindUserMenuAsync(Base_User user, Base_Menu menu, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(menu);
        return await UserMenus.FindAsync([user.Id, menu.Id], cancellationToken);
    }

    /// <summary>
    /// 添加系统菜单授权
    /// </summary>
    /// <param name="user">用户实例</param>
    /// <param name="menu">系统菜单实例</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task AddMenuToUserAsync(Base_User user, Base_Menu menu, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(menu);
        var userMenu = CreateUserMenu(user, menu);
        UserMenus.Add(userMenu);
        await SaveChanges(cancellationToken);
        await LogStore.WriteForCreateAsync(userMenu, OperationLogType.Create, cancellationToken);
    }

    protected Base_UserMenu CreateUserMenu(Base_User user, Base_Menu menu)
    {
        var authUser = Identity.GetSignUser();
        var userMenu = new Base_UserMenu
        {
            UserId = user.Id,
            MenuId = menu.Id,

        };
        userMenu.SetCreate(authUser);
        return userMenu;
    }

    /// <summary>
    /// 移除系统菜单授权
    /// </summary>
    /// <param name="user">用户实例</param>
    /// <param name="menu">系统菜单实例</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task RemoveMenuFromUserAsync(Base_User user, Base_Menu menu, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(menu);
        var userMenu = await UserMenus.FindAsync([user.Id, menu.Id], cancellationToken);
        if (userMenu != null)
        {
            UserMenus.Remove(userMenu);
            await SaveChanges(cancellationToken);
            await LogStore.WriteForRemoveAsync(userMenu, cancellationToken);
        }
    }


    /// <summary>
    /// 操作按钮是否授权给用户
    /// </summary>
    /// <param name="user">用户实例</param>
    /// <param name="buttonId">操作按钮主键</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> IsButtonInUserAsync(Base_User user, string buttonId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentException.ThrowIfNullOrWhiteSpace(buttonId);
        return await UserButtons.AnyAsync(rb => rb.UserId.Equals(user.Id) && rb.ButtonId.Equals(buttonId), cancellationToken);
    }

    /// <summary>
    /// 查询单个用户授权操作按钮
    /// </summary>
    /// <param name="user">用户实例</param>
    /// <param name="buttonId">操作按钮主键</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Base_UserButton?> FindUserButtonAsync(Base_User user, string buttonId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentException.ThrowIfNullOrWhiteSpace(buttonId);
        return await UserButtons.FindAsync([user.Id, buttonId], cancellationToken);
    }

    /// <summary>
    /// 添加操作按钮授权
    /// </summary>
    /// <param name="user">用户实例</param>
    /// <param name="buttonId">操作按钮主键</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task AddButtonToUserAsync(Base_User user, string buttonId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentException.ThrowIfNullOrWhiteSpace(buttonId);
        var userButton = CreateUserButton(user, buttonId);
        UserButtons.Add(userButton);
        await SaveChanges(cancellationToken);
        await LogStore.WriteForCreateAsync(userButton, OperationLogType.Create, cancellationToken);
    }

    protected Base_UserButton CreateUserButton(Base_User user, string buttonId)
    {
        var authUser = Identity.GetSignUser();
        var userButton = new Base_UserButton
        {
            UserId = user.Id,
            ButtonId = buttonId
        };
        userButton.SetCreate(authUser);
        return userButton;
    }


    /// <summary>
    /// 移除操作按钮授权
    /// </summary>
    /// <param name="user">用户实例</param>
    /// <param name="buttonId">操作按钮主键</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task RemoveButtonFromUserAsync(Base_User user, string buttonId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentException.ThrowIfNullOrWhiteSpace(buttonId);
        var userButton = await UserButtons.FindAsync([user, buttonId], cancellationToken);
        if (userButton != null)
        {
            UserButtons.Remove(userButton);
            await SaveChanges(cancellationToken);
            await LogStore.WriteForRemoveAsync(userButton, cancellationToken);
        }
    }

    /// <summary>
    /// 查询用户所有授权操作按钮
    /// </summary>
    /// <param name="user">用户实例</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IList<Base_Button>?> FindButtonsAsync(Base_User user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        var query = from button in Buttons
                    join userButton in UserButtons on button.Id equals userButton.ButtonId
                    where !button.IsDeleted && userButton.UserId.Equals(user.Id)
                    select button;
        return await query.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// 查询某个系统菜单的所有授权操作按钮
    /// </summary>
    /// <param name="user">用户实例</param>
    /// <param name="menu">系统菜单实例</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IList<OperationButtonDto>?> FindButtonsAsync(Base_User user, Base_Menu menu, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(menu);
        var query = from button in Buttons
                    join userButton in UserButtons on button.Id equals userButton.ButtonId
                    where !button.IsDeleted && userButton.UserId.Equals(user.Id) && button.MenuId.Equals(menu.Id)
                    select new OperationButtonDto
                    {
                        Id = button.Id,
                        Name = button.Name,
                        Title = button.Title,
                        Icon = button.Icon,
                        JSEvent = button.JSEvent,
                        Css = button.Css,
                        Description = button.Description,
                        Area = button.Area,
                        NormalizedArea = button.NormalizedArea,
                        MenuId = button.MenuId,
                        Url = button.Url,
                        NormalizedUrl = button.NormalizedUrl,
                        ButtonGroup = button.ButtonGroup,
                        ButtonType = button.ButtonType,
                        SortCode = button.SortCode,
                        IsRight = button.IsRight
                    };
        return await query.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// 查询某个系统菜单的所有授权操作按钮
    /// </summary>
    /// <param name="user">用户实例</param>
    /// <param name="menu">系统菜单实例</param>
    /// <param name="groups">操作按钮分组数组</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IList<OperationButtonDto>?> FindButtonsAsync(Base_User user, Base_Menu menu, ButtonGroup[]? groups = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(menu);
        var query = from button in Buttons
                    join userButton in UserButtons on button.Id equals userButton.ButtonId
                    where !button.IsDeleted && userButton.UserId.Equals(user.Id) && button.MenuId.Equals(menu.Id)
                    select new OperationButtonDto
                    {
                        Id = button.Id,
                        Name = button.Name,
                        Title = button.Title,
                        Icon = button.Icon,
                        JSEvent = button.JSEvent,
                        Css = button.Css,
                        Description = button.Description,
                        Area = button.Area,
                        NormalizedArea = button.NormalizedArea,
                        MenuId = button.MenuId,
                        Url = button.Url,
                        NormalizedUrl = button.NormalizedUrl,
                        ButtonGroup = button.ButtonGroup,
                        ButtonType = button.ButtonType,
                        SortCode = button.SortCode,
                        IsRight = button.IsRight
                    };
        if (groups != null)
        {
            var filter = ExpressionExtensions.False<OperationButtonDto>();
            foreach (var group in groups)
            {
                filter = filter.Or(b => b.ButtonGroup == group);
            }
            query = query.Where(filter);
        }
        return await query.ToListAsync(cancellationToken);
    }
    /// <summary>
    /// 是否有系统菜单授权
    /// </summary>
    /// <param name="user">登录用户 <see cref="Base_User"/></param>
    /// <param name="normalizedArea">区域标准值</param>
    /// <param name="normalizedPage">页面标准值</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<bool> HasMenuPermissionAsync(Base_User user, string? normalizedArea, string normalizedPage, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentException.ThrowIfNullOrWhiteSpace(normalizedPage);
        var query = from menu in Menus
                    join userMenu in UserMenus on menu.Id equals userMenu.MenuId
                    where !menu.IsDeleted && menu.NormalizedArea == normalizedArea && menu.NormalizedPage == normalizedPage && userMenu.UserId.Equals(user.Id)
                    select userMenu;
        return query.AnyAsync(cancellationToken);
    }


    /// <summary>
    /// 用户是否有指定操作按钮权限
    /// </summary>
    /// <param name="user">登录用户 <see cref="Base_User"/></param>
    /// <param name="normalizedArea">区域标准值</param>
    /// <param name="normalizedUrl">路径标准值</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<bool> HasButtonPermissionAsync(Base_User user, string? normalizedArea, string normalizedUrl, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentException.ThrowIfNullOrWhiteSpace(normalizedUrl);
        var query = from button in Buttons
                    join userButton in UserButtons on button.Id equals userButton.ButtonId
                    where !button.IsDeleted && button.NormalizedArea == normalizedArea && button.NormalizedUrl == normalizedUrl && userButton.UserId.Equals(user.Id)
                    select userButton;
        return query.AnyAsync(cancellationToken);
    }


    /// <summary>
    /// 获取某个指定授权操作按钮
    /// </summary>
    /// <param name="user">登录用户 <see cref="Base_User"/></param>
    /// <param name="area">区域</param>
    /// <param name="url">操作地址</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Base_Button?> GetPermitButtonAsync(Base_User user, string? normalizedArea, string normalizedUrl, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentException.ThrowIfNullOrWhiteSpace(normalizedUrl);
        var query = from button in Buttons
                    join userButton in UserButtons on button.Id equals userButton.ButtonId
                    where !button.IsDeleted && button.NormalizedArea == normalizedArea && button.NormalizedUrl == normalizedUrl && userButton.UserId.Equals(user.Id)
                    select button;
        return query.FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// 分页查询
    /// </summary>
    /// <param name="parameter"></param>
    /// <param name="roles"></param>
    /// <param name="isAdmin"></param>
    /// <param name="isDelete"></param>
    /// <param name="isLocked"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<DataTableResult<UserListDto>> FindPageAsync(DataTableParameter parameter, bool? isAdmin, bool? isDelete, bool? isLocked, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(parameter);

        var query = from user in Users
                    select new UserListDto
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        NormalizedUserName = user.NormalizedUserName,
                        RealName = user.RealName,
                        Email = user.Email,
                        NormalizedEmail = user.NormalizedEmail,
                        EmailConfirmed = user.EmailConfirmed,
                        PhoneNumber = user.PhoneNumber,
                        PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                        SecurityStamp = user.SecurityStamp,
                        PreviousAccess = user.PreviousAccess,
                        FirstAccess = user.FirstAccess,
                        LastAccess = user.LastAccess,
                        LockoutEnabled = user.LockoutEnabled,
                        LockoutEnd = user.LockoutEnd,
                        AccessCount = user.AccessCount,
                        AccessFailedCount = user.AccessFailedCount,
                        AccessIP = user.AccessIP,
                        IsAdministrator = user.IsAdministrator,
                        IsDeleted = user.IsDeleted,
                        Description = user.Description,
                        TwoFactorEnabled = user.TwoFactorEnabled,
                    };

        if (isAdmin != null)
        {
            query = query.Where(u => u.IsAdministrator == isAdmin);
        }
        if (isDelete != null)
        {
            query = query.Where(u => u.IsDeleted == isDelete);
        }
        if (isLocked != null)
        {
            query = query.Where(u => u.LockoutEnd < DateTimeOffset.Now);
        }
        var result = new DataTableResult<UserListDto>(parameter.Draw, await query.LongCountAsync(cancellationToken), 0, null);
        if (parameter.Search?.Value != null)
        {
            var searchPredicate = ExpressionExtensions.False<UserListDto>();
            foreach (var column in parameter.Columns)
            {
                if (column.Data != null && column.Searchable)
                {
                    searchPredicate.Or(ExpressionExtensions.Contains<UserListDto>(column.Data, parameter.Search.Value));
                }
            }
            query = query.Where(searchPredicate);
        }
        result.recordsFiltered = await query.LongCountAsync(cancellationToken);

        if (parameter.Order?.Count > 0)
        {
            int i = 0;
            foreach (var order in parameter.Order)
            {
                var column = parameter.Columns[order.Column];
                if (column.Data != null && column.Orderable)
                {
                    if (i == 0)
                    {
                        query = ExpressionExtensions.OrderBy(query, column.Data, order.Dir == OrderDirection.Desc ? true : false);
                    }
                    else
                    {
                        query = ExpressionExtensions.OrderByThen(query, column.Data, order.Dir == OrderDirection.Desc ? true : false);
                    }
                    ++i;
                }
            }
        }
        else
        {
            query = query.OrderBy(u => u.UserName);
        }

        query = query.Skip(parameter.Start).Take(parameter.Length);
        result.data = await query.ToListAsync(cancellationToken);
        return result;
    }



    /// <summary>
    /// 授权管理分页查询
    /// </summary>
    /// <param name="parameter"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<DataTableResult<UserPermitListDto>> FindPermitPageAsync(DataTableParameter parameter, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(parameter);
        var query = from user in Users
                    select new UserPermitListDto
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        RealName = user.RealName,
                        IsAdministrator = user.IsAdministrator,
                        IsDeleted = user.IsDeleted,
                        Description = user.Description
                    };
        return await ExpressionExtensions.FindPageAsync(query, parameter, cancellationToken: cancellationToken);
    }


    /// <summary>
    /// 获取系统导航系统菜单
    /// </summary>
    /// <param name="user">登录用户</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IList<Base_Menu>?> FindNavigationsAsync(Base_User user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        var query = from menu in Menus
                    join userMenu in UserMenus on menu.Id equals userMenu.MenuId
                    where !menu.IsDeleted && userMenu.UserId == user.Id && menu.Level <= 2
                    orderby menu.SortCode
                    select menu;
        return await query.ToListAsync(cancellationToken);
    }



    /// <summary>
    /// 是否有指定的操作权限
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="normalizedArea">区域标准值</param>
    /// <param name="normalizedUrl">操作路径标准值</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> HasOperationAsync(Base_User user, string? normalizedArea, string normalizedUrl, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentException.ThrowIfNullOrEmpty(normalizedUrl);
        var query = from button in Buttons
                    join ub in UserButtons on button.Id equals ub.ButtonId
                    where !button.IsDeleted && button.NormalizedArea == normalizedArea && button.NormalizedUrl == normalizedUrl
                    select ub;
        return await query.AnyAsync(cancellationToken);
    }
}
