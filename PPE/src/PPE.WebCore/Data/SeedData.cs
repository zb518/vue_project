using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using PPE.BLL;
using PPE.Core;
using PPE.DataModel;
using PPE.Model.Shared;
using PPE.ModelDto;
using System.Reflection;

namespace PPE.WebCore.Data;

public class SeedData
{

    public static List<IdentityError>? Errors { get; set; }

    private static void AddErrors(IdentityResult result)
    {
        if (!result.Succeeded)
        {
            Errors ??= new List<IdentityError>();
            Errors.AddRange(result.Errors);
        }
    }

    public static async Task Initialize(IServiceProvider service)
    {
        var context = service.GetRequiredService<CompleteDbContext>();
        ArgumentNullException.ThrowIfNull(context);
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        await AddMenus(service);
        await AddButtons(service);
        await AddRoles(service);
        await AddUsers(service);

        await AddMajors(service);
        await AddCourses(service);
        await AddCourseCatalogues(service);
    }

    private static async Task AddMenus(IServiceProvider service)
    {
        var menuManager = service.GetRequiredService<MenuManager>();
        var models = ReadModels<MenuImportDto>("系统菜单");
        if (models?.Count > 0)
        {
            var result = await menuManager.ImportModelsAsync(models);
            AddErrors(result);
        }
    }

    private static async Task AddButtons(IServiceProvider service)
    {
        var buttonManager = service.GetRequiredService<ButtonManager>();
        ArgumentNullException.ThrowIfNull(buttonManager);
        var models = ReadModels<ButtonImportDto>("操作按钮");
        if (models?.Count > 0)
        {
            foreach (var model in models)
            {
                AddErrors(await buttonManager.ImportButtonAsync(model));
            }
        }
    }

    private static async Task AddRoles(IServiceProvider service)
    {
        var roleManager = service.GetRequiredService<RoleManager>();
        ArgumentNullException.ThrowIfNull(roleManager);
        var role = new Base_Role(PermissionPolicies.AdministratorsRequirement, "系统管理员组");
        AddErrors(await roleManager.CreateAsync(role));
        var menuManager = service.GetRequiredService<MenuManager>();
        ArgumentNullException.ThrowIfNull(menuManager);
        var menus = menuManager.Entities.OrderBy(m => m.SortCode).Select(m => m.Id).ToList();
        if (menus?.Count > 0)
        {
            AddErrors(await roleManager.AddMenusToRoleAsync(role, menus!));
        }
        var buttonManager = service.GetRequiredService<ButtonManager>();
        ArgumentNullException.ThrowIfNull(buttonManager);
        var buttons = buttonManager.Entities.OrderBy(b => b.SortCode).Select(b => b.Id).ToList();
        if (buttons?.Count > 0)
        {
            AddErrors(await roleManager.AddButtonsToRoleAsync(role, buttons));
        }
    }

    private static async Task AddUsers(IServiceProvider service)
    {
        var userManager = service.GetService<UserManager>();
        ArgumentNullException.ThrowIfNull(userManager);
        var user = new Base_User
        {
            UserName = "Admin",
            RealName = "系统管理员",
            IsAdministrator = true,
            Description = "系统管理员"
        };
        AddErrors(await userManager.CreateAsync(user, "admin"));
        AddErrors(await userManager.AddToRoleAsync(user, PermissionPolicies.AdministratorsRequirement));
        var menuManager = service.GetRequiredService<MenuManager>();
        ArgumentNullException.ThrowIfNull(menuManager);
        var menus = menuManager.Entities.OrderBy(m => m.SortCode).Select(m => m.Id).ToList();
        if (menus?.Count > 0)
        {
            AddErrors(await userManager.AddMenusToUserAsync(user, menus!));
        }
        var buttonManager = service.GetRequiredService<ButtonManager>();
        ArgumentNullException.ThrowIfNull(buttonManager);
        var buttons = buttonManager.Entities.OrderBy(b => b.SortCode).Select(b => b.Id).ToList();
        if (buttons?.Count > 0)
        {
            AddErrors(await userManager.AddButtonsToUserAsync(user, buttons));
        }
    }



    private static async Task AddMajors(IServiceProvider service)
    {
        var manager = service.GetRequiredService<MajorManager>();
        ArgumentNullException.ThrowIfNull(manager);
        var models = ReadModels<MajorImportDto>("专业");
        if (models?.Count > 0)
        {
            AddErrors(await manager.ImportMajorAsync(models));
        }
    }

    private static async Task AddCourses(IServiceProvider service)
    {
        var manager = service.GetRequiredService<CourseManager>();
        ArgumentNullException.ThrowIfNull(manager);
        var models = ReadModels<CourseImportDto>("课程");
        if (models?.Count > 0)
        {
            AddErrors(await manager.ImportCoursesAsync(models));
        }
        var courseMajors = ReadModels<CourseMajorDto>("课程");
        if (courseMajors?.Count > 0)
        {
            AddErrors(await manager.ImportCoureMajorsAsync(courseMajors));
        }
    }

    private static async Task AddCourseCatalogues(IServiceProvider service)
    {
        var manager = service.GetRequiredService<CourseCatalogueManager>();
        ArgumentNullException.ThrowIfNull(manager);
        var models = ReadModels<CourseCatalogueImportDto>("课程目录");
        if (models?.Count > 0)
        {
            AddErrors(await manager.ImportCourseCataloguesAsync(models));
        }
    }
    private static IList<T>? ReadModels<T>(string excelFileName)
    {
        var dirPath = AppContext.BaseDirectory;
        dirPath = dirPath.Substring(0, dirPath.LastIndexOf("\\bin"));
        dirPath = dirPath.Substring(0, dirPath.LastIndexOf("\\"));
        dirPath = Path.Combine(dirPath, "src", "PPE.WebCore", "excel_files");
        var excelFile = Path.Combine(dirPath, $"{excelFileName}.xlsx");
        List<IdentityError>? errors = null;
        var models = ExcelHelper.ReadExcelBySheetName<T>(excelFile, typeof(T).GetCustomAttribute<SheetAttribute>()!.Name, errors);
        return models;
    }
}