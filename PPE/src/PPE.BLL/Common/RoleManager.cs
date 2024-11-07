using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PPE.Core;
using PPE.DataModel;
using PPE.IDAL;
using PPE.Model.Shared;
using PPE.Utilities;
using System.Security.Claims;
using System.Text;

namespace PPE.BLL;

/// <summary>
/// 角色业务逻辑管理
/// </summary>
public class RoleManager : RoleManager<Base_Role>
{
    public RoleManager(IRoleRepository store, IEnumerable<IRoleValidator<Base_Role>> roleValidators, ILookupNormalizer keyNormalizer, OperationErrorDescriber errors, ILogger<RoleManager> logger, IServiceProvider service) : base(store, roleValidators, keyNormalizer, errors, logger)
    {
        Store = store;
        Service = service;
        ErrorDescriber = errors ?? new OperationErrorDescriber();
    }
    public new IRoleRepository Store { get; set; }
    public IServiceProvider Service { get; private set; }
    public new OperationErrorDescriber ErrorDescriber { get; set; }
    public IdentityFactory Identity => Store.Identity;

    public override async Task<IdentityResult> AddClaimAsync(Base_Role role, Claim claim)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(claim);
        ArgumentNullException.ThrowIfNull(role);

        await Store.AddClaimAsync(role, claim, CancellationToken).ConfigureAwait(false);
        return IdentityResult.Success;
    }

    public async Task<string> FindPageAsync(DataTableParameter parameter)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(parameter);
        var result = await Store.FindPageAsync(parameter, CancellationToken);
        return JsonHelper.ConvertToJson(result);
    }

    public override async Task<IdentityResult> RemoveClaimAsync(Base_Role role, Claim claim)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(claim);
        ArgumentNullException.ThrowIfNull(role);

        await Store.RemoveClaimAsync(role, claim, CancellationToken).ConfigureAwait(false);
        return IdentityResult.Success;
    }
    protected IMenuRepository GetMenuStore()
    {
        var menuStroe = Service.GetRequiredService<IMenuRepository>();
        ArgumentNullException.ThrowIfNull(menuStroe);
        return menuStroe;
    }

    protected IButtonRepository GetButtonStore()
    {
        var buttonStroe = Service.GetRequiredService<IButtonRepository>();
        ArgumentNullException.ThrowIfNull(buttonStroe);
        return buttonStroe;
    }

    public async Task<string> BuildPermitTreeAsync(string roleName, string? menuId)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(roleName);
        var role = await FindByNameAsync(roleName).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(role);
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
                builder.AppendFormat(",\"checked\":{0}", (await Store.IsMenuInRoleAsync(role, menu, CancellationToken).ConfigureAwait(false)).ToString().ToLower());
                if (menu.Icon != null)
                {
                    builder.AppendFormat(",\"icon\":\"/images/icons/Icon16/{0}\"", menu.Icon);
                }
                if (await GetMenuStore().AnyAsync(m => m.ParentId.Equals(menu.Id), CancellationToken).ConfigureAwait(false))
                {
                    builder.AppendFormat(",\"children\":{0}", await BuildPermitTreeAsync(roleName, menu.Id).ConfigureAwait(false));
                }
                if (await GetButtonStore().AnyAsync(b => b.MenuId.Equals(menu.Id), CancellationToken).ConfigureAwait(false))
                {
                    builder.AppendFormat(",\"children\":{0}", await BuildButtonsTreeAsync(role, menu.Id).ConfigureAwait(false));
                }
                builder.Append("},");
            }
            builder = builder.Remove(builder.Length - 1, 1);
            return string.Format("[{0}]", builder);
        }
        return "[]";
    }

    private async Task<string> BuildButtonsTreeAsync(Base_Role role, string menuId)
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
            builder.AppendFormat(",\"checked\":{0}", (await Store.IsButtonInRoleAsync(role, btn, CancellationToken).ConfigureAwait(false)).ToString().ToLower());
            builder.Append("},");
        }
        builder = builder.Remove(builder.Length - 1, 1);
        builder.Append("]");
        return builder.ToString();
    }

    /// <summary>
    /// 添加单个系统菜单授权
    /// </summary>
    /// <param name="role">角色实体对象</param>
    /// <param name="menu">系统菜单主键</param>
    /// <returns></returns>
    public async Task<IdentityResult> AddMenuToRoleAsync(Base_Role role, string menu)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(role);
        ArgumentException.ThrowIfNullOrWhiteSpace(menu);
        var menuEntity = await GetMenuStore().FindByIdAsync(menu, CancellationToken).ConfigureAwait(false);
        if (menuEntity == null)
        {
            throw new ArgumentOutOfRangeException($"系统菜单 {menu} 不存在");
        }
        if (!await Store.IsMenuInRoleAsync(role, menuEntity, CancellationToken).ConfigureAwait(false))
        {
            //throw new ArgumentOutOfRangeException($"系统菜单 {menu} 已经授权给角色 {role.Name}");

            await Store.AddMenuToRoleAsync(role, menuEntity, CancellationToken).ConfigureAwait(false);
        }
        return IdentityResult.Success;
    }

    /// <summary>
    /// 添加多个系统菜单授权
    /// </summary>
    /// <param name="role">角色实体对象</param>
    /// <param name="menus">系统菜单主键集合</param>
    /// <returns></returns>
    public async Task<IdentityResult> AddMenusToRoleAsync(Base_Role role, IEnumerable<string> menus)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(role);
        ArgumentNullException.ThrowIfNull(menus);
        List<IdentityError>? errors = null;
        foreach (var menu in menus)
        {
            var result = await AddMenuToRoleAsync(role, menu).ConfigureAwait(false);
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
    /// <param name="role">角色实体对象</param>
    /// <param name="menu">系统菜单名称</param>
    /// <returns></returns>
    public async Task<IdentityResult> RemoveMenuFromRoleAsync(Base_Role role, string menu)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(role);
        ArgumentException.ThrowIfNullOrWhiteSpace(menu);
        var menuEntity = await GetMenuStore().FirstOrDefaultAsync(m => m.NormalizedName == NormalizeKey(menu), CancellationToken).ConfigureAwait(false);
        if (menuEntity == null)
        {
            throw new ArgumentOutOfRangeException($"系统菜单 {menu} 不存在");
        }
        if (await Store.IsMenuInRoleAsync(role, menuEntity, CancellationToken).ConfigureAwait(false))
        {
            //throw new ArgumentOutOfRangeException($"系统菜单 {menu} 已经授权给角色 {role.Name}");
            await Store.RemoveMenuFromRoleAsync(role, menuEntity, CancellationToken).ConfigureAwait(false);
        }
        return IdentityResult.Success;
    }

    /// <summary>
    /// 添加多个系统菜单授权
    /// </summary>
    /// <param name="role">角色实体对象</param>
    /// <param name="menus">系统菜单名称集合</param>
    /// <returns></returns>
    public async Task<IdentityResult> RemoveMenusFromRoleAsync(Base_Role role, IEnumerable<string> menus)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(role);
        ArgumentNullException.ThrowIfNull(menus);
        List<IdentityError>? errors = null;
        foreach (var menu in menus)
        {
            var result = await RemoveMenuFromRoleAsync(role, menu).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                errors ??= new List<IdentityError>();
                errors.AddRange(result.Errors);
            }
        }
        return errors?.Count > 0 ? IdentityResult.Failed(errors.ToArray()) : IdentityResult.Success;
    }

    /// <summary>
    /// 添加操作按钮授权
    /// </summary>
    /// <param name="role">角色实体</param>
    /// <param name="buttonId">操作按钮主键</param>
    /// <returns></returns>
    public async Task<IdentityResult> AddButtonToRoleAsync(Base_Role role, string buttonId)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(role);
        ArgumentException.ThrowIfNullOrWhiteSpace(buttonId);
        var button = await GetButtonStore().FindByIdAsync(buttonId, CancellationToken).ConfigureAwait(false);
        if (button == null)
        {
            throw new InvalidOperationException($"Button id {buttonId}  not found");
        }
        if (await Store.IsButtonInRoleAsync(role, button, CancellationToken).ConfigureAwait(false))
        {
            //return IdentityResult.Failed(ErrorDescriber.ButtonAlreadyInRole(buttonId, role.Name));
            Logger.LogDebug($"Button id {buttonId} is not a role.");
        }
        else
        {
            await Store.AddButtonToRoleAsync(role, button, CancellationToken).ConfigureAwait(false);
        }
        return IdentityResult.Success;
    }

    /// <summary>
    /// 添加多个操作按钮授权
    /// </summary>
    /// <param name="role">角色实体</param>
    /// <param name="buttons">操作按钮主键集合</param>
    /// <returns></returns>
    public async Task<IdentityResult> AddButtonsToRoleAsync(Base_Role role, IEnumerable<string> buttons)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(role);
        ArgumentNullException.ThrowIfNull(buttons);
        List<IdentityError>? errors = null;
        foreach (var button in buttons)
        {
            var result = await AddButtonToRoleAsync(role, button).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                errors ??= new List<IdentityError>();
                errors.AddRange(result.Errors);
            }
        }
        return errors?.Count > 0 ? IdentityResult.Failed(errors.ToArray()) : IdentityResult.Success;
    }



    /// <summary>
    /// 移除单个操作按钮授权
    /// </summary>
    /// <param name="role">角色实体</param>
    /// <param name="buttonId">操作按钮主键</param>
    /// <returns></returns>
    public async Task<IdentityResult> RemoveButtonFromRoleAsync(Base_Role role, string buttonId)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(role);
        ArgumentException.ThrowIfNullOrWhiteSpace(buttonId);
        var button = await GetButtonStore().FindByIdAsync(buttonId, CancellationToken).ConfigureAwait(false);
        if (button == null)
        {
            throw new InvalidOperationException($"Button id {buttonId}  not found");
        }
        if (await Store.IsButtonInRoleAsync(role, button, CancellationToken).ConfigureAwait(false))
        {
            //return IdentityResult.Failed(ErrorDescriber.ButtonAlreadyInRole(buttonId, role.Name));
            await Store.RemoveButtonFromRoleAsync(role, button, CancellationToken).ConfigureAwait(false);
        }
        return IdentityResult.Success;
    }

    /// <summary>
    /// 移除多个操作按钮授权
    /// </summary>
    /// <param name="role">角色实体</param>
    /// <param name="buttons">操作按钮主键集合</param>
    /// <returns></returns>
    public async Task<IdentityResult> RemoveButtonsFromRoleAsync(Base_Role role, IEnumerable<string> buttons)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(role);
        ArgumentNullException.ThrowIfNull(buttons);
        List<IdentityError>? errors = null;
        foreach (var button in buttons)
        {
            var result = await RemoveButtonFromRoleAsync(role, button).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                errors ??= new List<IdentityError>();
                errors.AddRange(result.Errors);
            }
        }
        return errors?.Count > 0 ? IdentityResult.Failed(errors.ToArray()) : IdentityResult.Success;
    }

    /// <summary>
    /// 恢复删除角色
    /// </summary>
    /// <param name="role">角色实体对象</param>
    /// <returns></returns>
    public async Task<IdentityResult> RecoveryAsync(Base_Role role)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(role);
        if (role.NormalizedName == NormalizeKey("Administrators"))
        {
            return IdentityResult.Failed(ErrorDescriber.RoleCannotDelete(role.Name!));
        }
        if (!role.IsDeleted)
        {
            return IdentityResult.Failed(ErrorDescriber.RoleNotDeleted(role.Name!));
        }
        return await Store.RecoveryAsync(role, CancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// 移除角色，移除后角色不存在
    /// </summary>
    /// <param name="role"></param>
    /// <returns></returns>
    public async Task<IdentityResult> RemoveAsync(Base_Role role)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(role);
        if (role.NormalizedName == NormalizeKey("Administrators"))
        {
            return IdentityResult.Failed(ErrorDescriber.RoleCannotDelete(role.Name!));
        }
        if (!role.IsDeleted)
        {
            return IdentityResult.Failed(ErrorDescriber.RoleNotDeleted(role.Name!));
        }
        return await Store.RemoveAsync(role, CancellationToken).ConfigureAwait(false);
    }
}