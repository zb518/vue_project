using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PPE.Core;
using PPE.DataModel;
using PPE.IDAL;
using PPE.Model.Shared;
using PPE.ModelDto;
using PPE.Utilities;

namespace PPE.BLL;
/// <summary>
/// 系统菜单业务逻辑管理
/// </summary>
public class MenuManager : BaseManager<Base_Menu, CommonDbContext>
{
    public MenuManager(IServiceProvider service, OperationErrorDescriber describer, IMenuRepository repository, ILogger<MenuManager> logger) : base(service, describer, repository, logger)
    {
        Store = repository;
    }

    public new IMenuRepository Store { get; set; }


    public override async Task<IdentityResult> ValidateAsync(Base_Menu menu)
    {
        var result = await base.ValidateAsync(menu).ConfigureAwait(false);
        if (!result.Succeeded)
        {
            return result;
        }
        if (menu.Name == null)
        {
            return IdentityResult.Failed(ErrorDescriber.CannotNullError("系统菜单名称"));
        }
        if (await Store.AnyAsync(m => !m.Id.Equals(menu.Id) && m.NormalizedName == menu.NormalizedName, CancellationToken).ConfigureAwait(false))
        {
            return IdentityResult.Failed(ErrorDescriber.DuplicateError($"系统菜单代码 {menu.Name}"));
        }
        if (await Store.AnyAsync(m => !m.Id.Equals(menu.Id) && m.NormalizedArea == menu.NormalizedArea && m.NormalizedPage == menu.NormalizedPage, CancellationToken).ConfigureAwait(false))
        {
            return IdentityResult.Failed(ErrorDescriber.DuplicateError($"区域 {menu.Area} 页面 {menu.Page}"));
        }
        return IdentityResult.Success;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<IdentityResult> ImportModelAsync(MenuImportDto model)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(model);
        try
        {
            var menu = MapperHelper.Mapper<Base_Menu, MenuImportDto>(source: model);
            if (model.ParentName != null)
            {
                var parent = await FindByNameAsync(model.ParentName).ConfigureAwait(false);
                if (parent == null)
                {
                    return IdentityResult.Failed(ErrorDescriber.NotExistsError($"系统菜单代码 {model.ParentName}"));
                }
                menu.ParentId = parent.Id;
            }
            else
            {
                menu.ParentId = Guid.Empty.ToString();
            }
            await Store.SetSortCodeAsync(menu, CancellationToken).ConfigureAwait(false);
            await SetLevelAsync(menu).ConfigureAwait(false);
            return await ImportAsync(menu).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, ex.Message);
            return IdentityResult.Failed(ErrorDescriber.DefaultError());
        }
    }

    /// <summary>
    /// 设置级数
    /// </summary>
    /// <param name="menu"></param>
    /// <returns></returns>
    public async Task SetLevelAsync(Base_Menu menu)
    {
        ThrowIfDisposed();
        if (menu.ParentId == Guid.Empty.ToString())
        {
            menu.Level = 1;
        }
        else
        {
            var level = await Store.GetLevelAsync(menu.ParentId, CancellationToken).ConfigureAwait(false);
            menu.Level = level++;
        }
    }

    /// <summary>
    /// 名称查询
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Task<Base_Menu?> FindByNameAsync(string name)
    {
        ThrowIfDisposed();
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        var normalizedName = NormalizedData(name);
        return Store.FirstOrDefaultAsync(m => m.NormalizedName == normalizedName, CancellationToken);
    }

    /// <summary>
    /// 导入系统菜单
    /// </summary>
    /// <param name="models"></param>
    /// <returns></returns>
    public async Task<IdentityResult> ImportModelsAsync(IEnumerable<MenuImportDto> models)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(models);
        List<IdentityError>? errors = null;
        foreach (var model in models)
        {
            var result = await ImportModelAsync(model).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                errors ??= new List<IdentityError>();
                errors.AddRange(result.Errors);
            }
        }
        return errors?.Count > 0 ? IdentityResult.Failed(errors.ToArray()) : IdentityResult.Success;
    }

    /// <summary>
    /// 构建 ztree 数据
    /// </summary>
    /// <param name="parentId">上级主键</param>
    /// <returns></returns>
    public async Task<string> BuildZTreeAsync(string? parentId)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append('[');
        var menus = await Store.FindListAsync(m => m.ParentId == parentId, "SortCode", false, CancellationToken).ConfigureAwait(false);
        if (menus?.Count > 0)
        {
            foreach (var menu in menus)
            {
                builder.Append('{');
                builder.AppendFormat("\"id\":\"{0}\"", menu.Id);
                builder.AppendFormat(",\"name\":\"{0}\"", menu.Name);
                builder.AppendFormat(",\"title\":\"{0}\"", menu.Title);
                builder.AppendFormat(",\"area\":\"{0}\"", menu.Area);
                builder.AppendFormat(",\"page\":\"{0}\"", menu.Page);
                builder.AppendFormat(",\"isDeleted\":{0}", menu.IsDeleted ? "true" : "false");
                if (await Store.AnyAsync(m => m.ParentId == menu.Id, CancellationToken).ConfigureAwait(false))
                {
                    builder.AppendFormat(",\"children\":{0}", await BuildZTreeAsync(menu.Id).ConfigureAwait(false));
                }
                builder.Append("},");
            }
            builder.Remove(builder.Length - 1, 1);
        }
        builder.Append(']');
        return builder.ToString();
    }


    /// <summary>
    /// 分页查询系统菜单
    /// </summary>
    /// <param name="parameter">分页查询参数 <see cref="DataTableParameter"/></param>
    /// <param name="parentId">上级主键</param>
    /// <returns></returns>
    public async Task<string> FindPageAsync(DataTableParameter parameter, string? parentId)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(parameter);
        var result = await Store.FindPageAsync(parameter, parentId, CancellationToken).ConfigureAwait(false);
        return JsonHelper.ConvertToJson(result);
    }
}
