using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PPE.Core;
using PPE.DataModel;
using PPE.IDAL;
using PPE.Model.Shared;
using PPE.ModelDto;
using PPE.Utilities;
using System.Security.Claims;
using System.Text;

namespace PPE.BLL;
/// <summary>
/// 用户业务逻辑管理
/// </summary>
public class UserManager : UserManager<Base_User>
{
    public UserManager(IUserRepository store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<Base_User> passwordHasher, IEnumerable<IUserValidator<Base_User>> userValidators, IEnumerable<IPasswordValidator<Base_User>> passwordValidators, ILookupNormalizer keyNormalizer, OperationErrorDescriber errors, IServiceProvider services, ILogger<UserManager> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    {
        Store = store;
        ErrorDescriber = errors ?? new OperationErrorDescriber();
        Service = services;
    }
    public new IUserRepository Store { get; set; }
    public new OperationErrorDescriber ErrorDescriber { get; set; }
    public IServiceProvider Service { get; private set; }

    public IdentityFactory Identity => Store.Identity;


    public override async Task<IdentityResult> AddToRoleAsync(Base_User user, string role)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);

        var normalizedRole = NormalizeName(role);
        if (await Store.IsInRoleAsync(user, normalizedRole, CancellationToken).ConfigureAwait(false))
        {
            return UserAlreadyInRoleError(role);
        }
        await Store.AddToRoleAsync(user, normalizedRole, CancellationToken).ConfigureAwait(false);
        return IdentityResult.Success;
    }

    public override async Task<IdentityResult> AddToRolesAsync(Base_User user, IEnumerable<string> roles)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(roles);

        foreach (var role in roles.Distinct())
        {
            var normalizedRole = NormalizeName(role);
            if (await Store.IsInRoleAsync(user, normalizedRole, CancellationToken).ConfigureAwait(false))
            {
                return UserAlreadyInRoleError(role);
            }
            await Store.AddToRoleAsync(user, normalizedRole, CancellationToken).ConfigureAwait(false);
        }
        return IdentityResult.Success;
    }

    private IdentityResult UserAlreadyInRoleError(string role)
    {
        if (Logger.IsEnabled(LogLevel.Debug))
        {
            Logger.LogDebug(LoggerEventIds.UserAlreadyInRole, "User is already in role {role}.", role);
        }
        return IdentityResult.Failed(ErrorDescriber.UserAlreadyInRole(role));
    }

    private IdentityResult UserNotInRoleError(string role)
    {
        if (Logger.IsEnabled(LogLevel.Debug))
        {
            Logger.LogDebug(LoggerEventIds.UserNotInRole, "User is not in role {role}.", role);
        }
        return IdentityResult.Failed(ErrorDescriber.UserNotInRole(role));
    }

    public override async Task<IdentityResult> RemoveFromRoleAsync(Base_User user, string role)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);

        var normalizedRole = NormalizeName(role);
        if (!await Store.IsInRoleAsync(user, normalizedRole, CancellationToken).ConfigureAwait(false))
        {
            return UserNotInRoleError(role);
        }
        await Store.RemoveFromRoleAsync(user, normalizedRole, CancellationToken).ConfigureAwait(false);
        return IdentityResult.Success;
    }


    public override async Task<IdentityResult> RemoveFromRolesAsync(Base_User user, IEnumerable<string> roles)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(roles);

        foreach (var role in roles)
        {
            var normalizedRole = NormalizeName(role);
            if (!await Store.IsInRoleAsync(user, normalizedRole, CancellationToken).ConfigureAwait(false))
            {
                return UserNotInRoleError(role);
            }
            await Store.RemoveFromRoleAsync(user, normalizedRole, CancellationToken).ConfigureAwait(false);
        }
        return IdentityResult.Success;
    }

    public override async Task<IdentityResult> AddLoginAsync(Base_User user, UserLoginInfo login)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(login);
        ArgumentNullException.ThrowIfNull(user);

        var existingUser = await FindByLoginAsync(login.LoginProvider, login.ProviderKey).ConfigureAwait(false);
        if (existingUser != null)
        {
            Logger.LogDebug(LoggerEventIds.AddLoginFailed, "AddLogin for user failed because it was already associated with another user.");
            return IdentityResult.Failed(ErrorDescriber.LoginAlreadyAssociated());
        }
        await Store.AddLoginAsync(user, login, CancellationToken).ConfigureAwait(false);
        return IdentityResult.Success;
    }

    private async Task UpdateSecurityStampInternal(Base_User user)
    {
        await Store.SetSecurityStampAsync(user, NewSecurityStamp(), CancellationToken).ConfigureAwait(false);
    }
    private static string NewSecurityStamp()
    {
#if NETSTANDARD2_0 || NETFRAMEWORK
        byte[] bytes = new byte[20];
        _rng.GetBytes(bytes);
        return Base32.ToBase32(bytes);
#else
        return Base32.GenerateBase32();
#endif
    }

    public override async Task<IdentityResult> AddClaimsAsync(Base_User user, IEnumerable<Claim> claims)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(claims);
        ArgumentNullException.ThrowIfNull(user);

        await Store.AddClaimsAsync(user, claims, CancellationToken).ConfigureAwait(false);
        return IdentityResult.Success;
    }

    public override async Task<IdentityResult> ReplaceClaimAsync(Base_User user, Claim claim, Claim newClaim)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(claim);
        ArgumentNullException.ThrowIfNull(newClaim);
        ArgumentNullException.ThrowIfNull(user);

        await Store.ReplaceClaimAsync(user, claim, newClaim, CancellationToken).ConfigureAwait(false);
        return IdentityResult.Success;
    }

    public override async Task<IdentityResult> RemoveClaimsAsync(Base_User user, IEnumerable<Claim> claims)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(claims);

        await Store.RemoveClaimsAsync(user, claims, CancellationToken).ConfigureAwait(false);
        return IdentityResult.Success;
    }



    /// <summary>
    /// 更新用户登录信息
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public Task<IdentityResult> UpdateLoginInfoAsync(Base_User user)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        user.AccessIP = Identity.GetIPAddress();
        user.AccessCount++;
        user.FirstAccess ??= DateTime.Now;
        user.PreviousAccess = user.LastAccess;
        user.LastAccess = DateTime.Now;
        return Store.UpdateLoginInfoAsync(user, CancellationToken);
    }

    private IMenuRepository GetMenuStore()
    {
        var menuStore = Service.GetRequiredService<IMenuRepository>();
        ArgumentNullException.ThrowIfNull(menuStore);
        return menuStore;
    }

    private IButtonRepository GetButtonStore()
    {
        var buttonStore = Service.GetRequiredService<IButtonRepository>();
        ArgumentNullException.ThrowIfNull(buttonStore);
        return buttonStore;
    }

    /// <summary>
    /// 添加单个系统菜单授权
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <param name="menu">系统菜单主键</param>
    /// <returns></returns>
    public async Task<IdentityResult> AddMenuToUserAsync(Base_User user, string menu)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentException.ThrowIfNullOrWhiteSpace(menu);
        var menuEntity = await GetMenuStore().FindByIdAsync(menu, CancellationToken).ConfigureAwait(false);
        if (menuEntity == null)
        {
            throw new InvalidOperationException($"系统菜单 {menu} 不正确");
        }
        if (!await Store.IsMenuInUserAsync(user, menuEntity, CancellationToken).ConfigureAwait(false))
        {
            //throw new InvalidOperationException($"系统菜单 {menuName} 已经授权组用户 {user.UserName}");

            await Store.AddMenuToUserAsync(user, menuEntity, CancellationToken).ConfigureAwait(false);
        }
        return IdentityResult.Success;
    }

    /// <summary>
    /// 添加多个系统菜单授权
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <param name="menus">系统菜单主键集合</param>
    /// <returns></returns>
    public async Task<IdentityResult> AddMenusToUserAsync(Base_User user, IEnumerable<string> menus)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(menus);
        List<IdentityError>? errors = null;
        foreach (var menu in menus)
        {
            var result = await AddMenuToUserAsync(user, menu).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                errors ??= new List<IdentityError>();
                errors.AddRange(result.Errors);
            }
        }
        return errors?.Count > 0 ? IdentityResult.Failed(errors.ToArray()) : IdentityResult.Success;
    }

    /// <summary>
    /// 移除单个系统菜单授权
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <param name="menuName">系统菜单名称</param>
    /// <returns></returns>
    public async Task<IdentityResult> RemoveMenuFromUserAsync(Base_User user, string menuName)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentException.ThrowIfNullOrWhiteSpace(menuName);
        var menu = await GetMenuStore().FirstOrDefaultAsync(m => m.NormalizedName == NormalizeName(menuName), CancellationToken).ConfigureAwait(false);
        if (menu == null)
        {
            throw new InvalidOperationException($"系统菜单 {menuName} 不正确");
        }
        if (await Store.IsMenuInUserAsync(user, menu, CancellationToken).ConfigureAwait(false))
        {
            // throw new InvalidOperationException($"系统菜单 {menuName} 未授权组用户 {user.UserName}");

            await Store.RemoveMenuFromUserAsync(user, menu, CancellationToken).ConfigureAwait(false);
        }
        return IdentityResult.Success;
    }

    /// <summary>
    /// 移除多个系统菜单授权
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <param name="menus">系统菜单名称列表</param>
    /// <returns></returns>
    public async Task<IdentityResult> RemoveMenusFromUserAsync(Base_User user, IEnumerable<string> menus)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(menus);
        List<IdentityError>? errors = null;
        foreach (var menu in menus)
        {
            var result = await RemoveMenuFromUserAsync(user, menu).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                errors ??= new List<IdentityError>();
                errors.AddRange(result.Errors);
            }
        }
        return errors?.Count > 0 ? IdentityResult.Failed(errors.ToArray()) : IdentityResult.Success;
    }

    /// <summary>
    /// 查询用户授权系统菜单
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <returns></returns>
    public Task<IList<Base_Menu>?> FindMenusAsync(Base_User user)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        return Store.FindMenusByUserAsync(user, CancellationToken);
    }

    /// <summary>
    /// 添加单个操作按钮授权
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <param name="buttonId">操作按钮主键</param>
    /// <returns></returns>
    public async Task<IdentityResult> AddButtonToUserAsync(Base_User user, string buttonId)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentException.ThrowIfNullOrWhiteSpace(buttonId);
        var button = await GetButtonStore().FindByIdAsync(buttonId, CancellationToken).ConfigureAwait(false);
        if (button == null)
        {
            throw new InvalidOperationException($"Button id {buttonId} does not exist.");
        }
        if (!await Store.IsButtonInUserAsync(user, button, CancellationToken).ConfigureAwait(false))
        {
            //throw new InvalidOperationException($"操作按钮 {buttonId} 已经授权给用户 {user.UserName}。");

            await Store.AddButtonToUserAsync(user, button, CancellationToken).ConfigureAwait(false);
        }
        return IdentityResult.Success;
    }

    /// <summary>
    /// 添加多个操作按钮授权
    /// /// </summary>
    /// <param name="user">用户实体</param>
    /// <param name="buttons">操作按钮主键列表</param>
    /// <returns></returns>
    public async Task<IdentityResult> AddButtonsToUserAsync(Base_User user, IEnumerable<string> buttons)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(buttons);
        List<IdentityError>? errors = null;
        foreach (var button in buttons)
        {
            var result = await AddButtonToUserAsync(user, button).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                errors ??= new List<IdentityError>();
                errors.AddRange(result.Errors);
            }
        }
        return errors?.Count > 0 ? IdentityResult.Failed(errors.ToArray()) : IdentityResult.Success;
    }

    /// <summary>
    /// 添加单个操作按钮授权
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <param name="buttonId">操作按钮主键</param>
    /// <returns></returns>
    public async Task<IdentityResult> RemoveButtonFromUserAsync(Base_User user, string buttonId)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentException.ThrowIfNullOrWhiteSpace(buttonId);
        var button = await GetButtonStore().FindByIdAsync(buttonId, CancellationToken).ConfigureAwait(false);
        if (button == null)
        {
            throw new InvalidOperationException($"Button id {buttonId} does not exist.");
        }
        if (!await Store.IsButtonInUserAsync(user, button, CancellationToken).ConfigureAwait(false))
        {
            // throw new InvalidOperationException($"操作按钮 {buttonId} 已经授权给用户 {user.UserName}。");

            await Store.RemoveButtonFromUserAsync(user, button, CancellationToken).ConfigureAwait(false);
        }
        return IdentityResult.Success;
    }

    /// <summary>
    /// 添加多个操作按钮授权
    /// /// </summary>
    /// <param name="user">用户实体</param>
    /// <param name="buttons">操作按钮主键列表</param>
    /// <returns></returns>
    public async Task<IdentityResult> RemoveButtonsFromUserAsync(Base_User user, IEnumerable<string> buttons)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(buttons);
        List<IdentityError>? errors = null;
        foreach (var button in buttons)
        {
            var result = await RemoveButtonFromUserAsync(user, button).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                errors ??= new List<IdentityError>();
                errors.AddRange(result.Errors);
            }
        }
        return errors?.Count > 0 ? IdentityResult.Failed(errors.ToArray()) : IdentityResult.Success;
    }

    /// <summary>
    /// 查询用户所有授权操作按钮
    /// </summary>
    /// <returns></returns>
    public async Task<IList<Base_Button>?> FindButtonsAsync()
    {
        ThrowIfDisposed();
        var user = await GetUserAsync(Identity.Context.User).ConfigureAwait(false);
        if (user == null)
        {
            return null;
        }
        return await Store.FindButtonsAsync(user, CancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// 查询指定路径下的系统菜单
    /// </summary>
    /// <param name="area">区域</param>
    /// <param name="url">页面地址</param>
    /// <returns></returns>
    public async Task<IList<OperationButtonDto>?> FindButtonsForPathAsync(string? area, string url)
    {
        ThrowIfDisposed();
        ArgumentException.ThrowIfNullOrWhiteSpace(url);
        var user = await GetUserAsync(Identity.Context.User).ConfigureAwait(false);
        if (user == null)
        {
            return null;
        }
        var menu = await GetMenuStore().FirstOrDefaultAsync(m => m.NormalizedArea == NormalizeName(area) && m.NormalizedPage == NormalizeName(url), CancellationToken).ConfigureAwait(false);
        if (menu == null)
        {
            return null;
        }
        return await Store.FindButtonsAsync(user, menu, CancellationToken).ConfigureAwait(false);
    }

    public async Task<IList<OperationButtonDto>?> FindButtonsForPathAsync(string? area, string url, ButtonGroup[]? groups = null)
    {
        ThrowIfDisposed();
        ArgumentException.ThrowIfNullOrWhiteSpace(url);
        var user = await GetUserAsync(Identity.Context.User).ConfigureAwait(false);
        if (user == null)
        {
            return null;
        }
        var menu = await GetMenuStore().FirstOrDefaultAsync(m => m.NormalizedArea == NormalizeName(area) && m.NormalizedPage == NormalizeName(url), CancellationToken).ConfigureAwait(false);
        if (menu == null)
        {
            throw new InvalidOperationException($"系统菜单区域 {area} 地址 {url} 不存在");
        }
        return await Store.FindButtonsAsync(user, menu, groups, CancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// 是否有系统菜单授权
    /// </summary>
    /// <param name="user">登录用户 <see cref="Base_User"/></param>
    /// <param name="area">区域</param>
    /// <param name="page">页面</param>
    /// <returns></returns>
    public async Task<bool> HasMenuPermissionAsync(Base_User? user, string? area, string page)
    {
        ThrowIfDisposed();
        if (user == null)
        {
            return false;
        }
        var normalizedArea = NormalizeName(area);
        var normalizedPage = NormalizeName(page);
        return await Store.HasMenuPermissionAsync(user, normalizedArea, normalizedPage, CancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// 用户是否有指定操作按钮权限
    /// </summary>
    /// <param name="user">登录用户 <see cref="Base_User"/></param>
    /// <param name="area">区域</param>
    /// <param name="url">操作地址</param>
    /// <returns></returns>
    public async Task<bool> HasButtonPermissionAsync(Base_User? user, string? area, string url)
    {
        ThrowIfDisposed();
        if (user == null)
        {
            return false;
        }
        var normalizedArea = NormalizeName(area);
        var normalizedUrl = NormalizeName(url);
        return await Store.HasButtonPermissionAsync(user, normalizedArea, normalizedUrl, CancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// 获取某个指定授权操作按钮
    /// </summary>
    /// <param name="user">登录用户 <see cref="Base_User"/></param>
    /// <param name="area">区域</param>
    /// <param name="url">操作地址</param>
    /// <returns></returns>
    public async Task<Base_Button?> GetPermitButtonAsync(Base_User? user, string? area, string url)
    {
        ThrowIfDisposed();
        if (user == null)
        {
            return null;
        }
        var normalizedArea = NormalizeName(area);
        var normalizedUrl = NormalizeName(url);
        return await Store.GetPermitButtonAsync(user, normalizedArea, normalizedUrl, CancellationToken).ConfigureAwait(false);
    }

    public async Task<string> FindPageAsync(DataTableParameter parameter, bool? isAdmin, bool? isDelete, bool? isLocked)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(parameter);
        var result = await Store.FindPageAsync(parameter, isAdmin, isDelete, isLocked, CancellationToken).ConfigureAwait(false);
        return JsonHelper.ConvertToJson(result);
    }


    public async Task<string> BuildPermitTreeAsync(string userName, string? menuId)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(userName);
        var user = await FindByNameAsync(userName).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(user);
        var menus = await GetMenuStore().FindListAsync(m => m.ParentId.Equals(menuId ?? Guid.Empty.ToString()) && !m.IsDeleted, "SortCode", false, CancellationToken).ConfigureAwait(false);
        if (menus?.Count > 0)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var menu in menus)
            {
                builder.Append('{');
                builder.AppendFormat("\"id\":\"{0}\"", menu.Id);
                builder.AppendFormat(",\"name\":\"{0}\"", menu.Name);
                builder.AppendFormat(",\"type\":\"{0}\"", 'M');
                builder.AppendFormat(",\"level\":\"{0}\"", menu.Level);
                builder.AppendFormat(",\"checked\":{0}", (await Store.IsMenuInUserAsync(user, menu, CancellationToken).ConfigureAwait(false)).ToString().ToLower());
                if (menu.Icon != null)
                {
                    builder.AppendFormat(",\"icon\":\"/images/icons/Icon16/{0}\"", menu.Icon);
                }
                if (await GetMenuStore().AnyAsync(m => m.ParentId.Equals(menu.Id), CancellationToken).ConfigureAwait(false))
                {
                    builder.AppendFormat(",\"children\":{0}", await BuildPermitTreeAsync(userName, menu.Id).ConfigureAwait(false));
                }
                if (await GetButtonStore().AnyAsync(b => b.MenuId.Equals(menu.Id), CancellationToken).ConfigureAwait(false))
                {
                    builder.AppendFormat(",\"children\":{0}", await BuildButtonsTreeAsync(user, menu.Id).ConfigureAwait(false));
                }
                builder.Append("},");
            }
            builder = builder.Remove(builder.Length - 1, 1);
            return string.Format("[{0}]", builder);
        }
        return "[]";
    }

    private async Task<string> BuildButtonsTreeAsync(Base_User user, string menuId)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("[");
        var buttons = await GetButtonStore().FindListAsync(b => b.MenuId.Equals(menuId) && !b.IsDeleted, CancellationToken).ConfigureAwait(false);
        foreach (var btn in buttons!)
        {
            builder.Append('{');
            builder.AppendFormat("\"id\":\"{0}\"", btn.Id);
            builder.AppendFormat(",\"name\":\"{0}\"", btn.Name);
            builder.AppendFormat(",\"type\":\"{0}\"", 'B');
            builder.AppendFormat(",\"checked\":{0}", (await Store.IsButtonInUserAsync(user, btn, CancellationToken).ConfigureAwait(false)).ToString().ToLower());
            builder.Append("},");
        }
        builder = builder.Remove(builder.Length - 1, 1);
        builder.Append("]");
        return builder.ToString();
    }

    public async Task<string> FindPermitPageAsync(DataTableParameter parameter)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(parameter);
        var result = await Store.FindPermitPageAsync(parameter, CancellationToken).ConfigureAwait(false);
        return JsonHelper.ConvertToJson(result);
    }

    /// <summary>
    /// 查询系统导航系统菜单
    /// </summary>
    /// <param name="user">登录用户</param>
    /// <returns></returns>
    public Task<IList<Base_Menu>?> FindNavigationsAsync(Base_User user)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        return Store.FindNavigationsAsync(user, CancellationToken);
    }


    public async Task<bool> HasEditAsync(string? area, string url)
    {
        ThrowIfDisposed();
        ArgumentException.ThrowIfNullOrWhiteSpace(url);
        var normalizedArea = NormalizeName(area);
        var normalizedUrl = NormalizeName(url);
        var user = await GetUserAsync(Identity.Context.User);
        if (user == null)
        {
            return false;
        }
        return await Store.HasOperationAsync(user, normalizedArea, normalizedUrl, CancellationToken);
    }
}