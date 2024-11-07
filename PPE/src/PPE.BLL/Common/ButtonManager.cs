using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PPE.DataModel;
using PPE.IDAL;
using PPE.Model.Shared;
using PPE.ModelDto;
using PPE.Utilities;

namespace PPE.BLL;

/// <summary>
/// 操作按钮业务逻辑管理
/// </summary>
public class ButtonManager : BaseManager<Base_Button, CommonDbContext>
{
    public ButtonManager(IServiceProvider service, OperationErrorDescriber describer, IButtonRepository repository, ILogger<BaseManager<Base_Button, CommonDbContext>> logger) : base(service, describer, repository, logger)
    {
        Store = repository;
    }

    public new IButtonRepository Store { get; set; }

    public override async Task<IdentityResult> ValidateAsync(Base_Button button)
    {
        var result = await base.ValidateAsync(button).ConfigureAwait(false);
        if (!result.Succeeded)
        {
            return result;
        }
        if (button.MenuId == null)
        {
            return IdentityResult.Failed(ErrorDescriber.InvalidError("系统菜单主键"));
        }
        return IdentityResult.Success;
    }

    /// <summary>
    /// 导入操作按钮
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<IdentityResult> ImportButtonAsync(ButtonImportDto dto)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(dto);
        var config = new MapperConfiguration(cfg => cfg.CreateMap<ButtonImportDto, Base_Button>());
        var button = config.CreateMapper().Map<Base_Button>(dto);
        if (string.IsNullOrWhiteSpace(dto.MenuName))
        {
            return IdentityResult.Failed(ErrorDescriber.CannotNullError("系统菜单名称"));
        }
        var menuId = await Store.FindMenuIdByNameAsync(dto.MenuName, CancellationToken).ConfigureAwait(false);
        if (menuId == null)
        {
            return IdentityResult.Failed(ErrorDescriber.NotExistsError($"系统菜单 {dto.MenuName}"));
        }
        button.MenuId = menuId;
        await SetSortCodeAsync(button).ConfigureAwait(false);
        return await ImportAsync(button).ConfigureAwait(false);
    }

    private async Task SetSortCodeAsync(Base_Button button)
    {
        ThrowIfDisposed();
        var maxSortCode = await Store.FindMaxSortCodeAsync(button.MenuId, CancellationToken).ConfigureAwait(false);
        if (maxSortCode == null)
        {
            var menu = await Store.FindMenuByIdAsync(button.MenuId, CancellationToken).ConfigureAwait(false);
            ArgumentNullException.ThrowIfNull(menu);
            button.SortCode = menu.SortCode + "01";
        }
        else
        {
            button.SortCode = string.Format("{0}", Convert.ToInt64(maxSortCode) + 1);
        }
    }



    //public Task<IList<Base_Button>> BulkAddButtons(Base_Menu menu)
    //{
    //	var groups = GetButtonGroupManager().Entities.OrderBy(g => g.SortCode).ToList();
    //	IList<Base_Button> buttons = new List<Base_Button>{
    //		new Base_Button{
    //			MenuId = menu.Id,
    //			Name = "添加",
    //			Title = "添加一个新记录",
    //			ButtonGroupId = groups.First(g=>g.NormalizedCode=="HEADER").Id,
    //			Css = "btn btn-sm btn-primary",
    //			Icon = "fa-solid fa-plus",
    //			Description = "添加一个新记录",
    //			SortCode = menu.SortCode+"01",
    //			Url = menu.Page!.Replace("/Index", "/"+"Create")
    //		},
    //		new Base_Button{
    //			MenuId = menu.Id,
    //			Name = "导入",
    //			Title = "导入记录",
    //			ButtonGroupId = groups.First(g=>g.NormalizedCode=="OTHER").Id,
    //			Css = "btn btn-sm btn-primary",
    //			Icon = "fa-solid fa-file-import",
    //			Description = "导入记录",
    //			SortCode = menu.SortCode+"02",
    //			Url = menu.Page!.Replace("/Index", "/"+"Import")
    //		},
    //		new Base_Button{
    //			MenuId = menu.Id,
    //			Name = "明细",
    //			Title = "查看详细信息",
    //			ButtonGroupId = groups.First(g=>g.NormalizedCode=="LIST").Id,
    //			Css = "btn btn-sm btn-primary",
    //			Icon = "fa-solid fa-list",
    //			Description = "查看详细信息",
    //			SortCode = menu.SortCode+"03",
    //			Url = menu.Page!.Replace("/Index", "/"+"Detail")
    //		},
    //		new Base_Button{
    //			MenuId = menu.Id,
    //			Name = "修改",
    //			Title = "修改记录",
    //			ButtonGroupId = groups.First(g=>g.NormalizedCode=="LIST").Id,
    //			Css = "btn btn-sm btn-primary",
    //			Icon = "fa-solid fa-edit",
    //			Description = "修改记录",
    //			SortCode = menu.SortCode+"04",
    //			Url = menu.Page!.Replace("/Index", "/"+"Edit")
    //		},
    //		new Base_Button{
    //			MenuId = menu.Id,
    //			Name = "删除",
    //			Title = "删除记录",
    //			ButtonGroupId = groups.First(g=>g.NormalizedCode=="LIST").Id,
    //			Css = "btn btn-sm btn-warning",
    //			Icon = "fa-solid fa-trash",
    //			Description = "删除记录，只做标记",
    //			SortCode = menu.SortCode+"05",
    //			JSEvent="Delete(this)",
    //			Url = menu.Page!.Replace("/Index", "/"+"Delete")
    //		},
    //		new Base_Button{
    //			MenuId = menu.Id,
    //			Name = "恢复",
    //			Title = "恢复删除记录",
    //			 ButtonGroupId = groups.First(g=>g.NormalizedCode=="LIST").Id,
    //			Css = "btn btn-sm btn-success",
    //			Icon = "fa-solid fa-rotate-left",
    //			Description = "恢复删除记录",
    //			SortCode = menu.SortCode+"06",
    //			JSEvent="Recovery(this)",
    //			Url = menu.Page!.Replace("/Index", "/"+"Recovery")
    //		},
    //		new Base_Button{
    //			MenuId = menu.Id,
    //			Name = "移除",
    //			Title = "移除删除记录",
    //			ButtonGroupId = groups.First(g=>g.NormalizedCode=="LIST").Id,
    //			Css = "btn btn-sm btn-danger",
    //			Icon = "fa-solid fa-minus",
    //			Description = "移除删除记录后，记录不存在",
    //			SortCode = menu.SortCode+"07",
    //			JSEvent="Remove(this)",
    //			Url = menu.Page!.Replace("/Index", "/"+"Remove")
    //		},
    //		new Base_Button{
    //			MenuId = menu.Id,
    //			Name = "导出",
    //			Title = "导出记录",
    //			ButtonGroupId = groups.First(g=>g.NormalizedCode=="OTHER").Id,
    //			Css = "btn btn-sm btn-primary",
    //			Icon = "fa-solid fa-file-export",
    //			Description = "导入记录",
    //			SortCode = menu.SortCode+"02",
    //			Url = menu.Page!.Replace("/Index", "/"+"Export")
    //		},
    //	};
    //	foreach (var btn in buttons)
    //	{
    //		btn.Area = menu.Area;
    //		btn.NormalizedArea = menu.NormalizedArea;
    //		btn.NormalizedUrl = NormalizedData(btn.Url);
    //	}
    //	return Task.FromResult(buttons);
    //}

    /// <summary>
    /// 分页查询
    /// </summary>
    /// <param name="parameter">分页查询参数 <see cref="DataTableParameter"/></param>
    /// <param name="menuId">系统菜单主键</param>
    /// <returns></returns>
    public async Task<string> FindPageAsync(DataTableParameter parameter, string menuId)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(parameter);
        ArgumentException.ThrowIfNullOrWhiteSpace(menuId);
        var result = await Store.FindPageAsync(parameter, menuId, CancellationToken).ConfigureAwait(false);
        return JsonHelper.ConvertToJson(result);
    }

    /// <summary>
    /// 查询系统菜单下所有操作按钮
    /// </summary>
    /// <param name="menuId"></param>
    /// <returns></returns>
    public Task<IList<Base_Button>?> FindByMenuIdAsync(string menuId)
    {
        ThrowIfDisposed();
        ArgumentException.ThrowIfNullOrWhiteSpace(menuId);
        return Store.FindListAsync(b => b.MenuId == menuId, "SortCode", false, CancellationToken);
    }
}