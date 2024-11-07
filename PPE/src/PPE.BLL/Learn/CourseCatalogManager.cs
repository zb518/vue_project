using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PPE.DataModel;
using PPE.IDAL;
using PPE.Model.Shared;
using PPE.ModelDto;
using PPE.Utilities;
using System.Text;

namespace PPE.BLL;

public class CourseCatalogueManager : BaseManager<Base_CourseCatalogue, LearnDbContext>
{
    public CourseCatalogueManager(IServiceProvider service, OperationErrorDescriber describer, ICourseCatalogueRepository repository, ILogger<CourseCatalogueManager> logger) : base(service, describer, repository, logger)
    {
        Store = repository;
    }
    public new ICourseCatalogueRepository Store { get; set; }

    public override async Task<IdentityResult> ValidateAsync(Base_CourseCatalogue catalogue)
    {
        var result = await base.ValidateAsync(catalogue).ConfigureAwait(false);
        if (!result.Succeeded)
        {
            return result;
        }
        if (catalogue.CourseId == null)
        {
            return IdentityResult.Failed(ErrorDescriber.InvalidError("课程"));
        }
        if (catalogue.Content == null)
        {
            return IdentityResult.Failed(ErrorDescriber.InvalidError("内容"));
        }
        if (await Store.AnyAsync(c => !c.Id.Equals(catalogue.Id) && c.CourseId == catalogue.CourseId && c.NormalizedContent == catalogue.NormalizedContent && c.ParentId == catalogue.ParentId, CancellationToken).ConfigureAwait(false))
        {
            return IdentityResult.Failed(ErrorDescriber.DuplicateError(catalogue.Content));
        }
        return IdentityResult.Success;
    }

    /// <summary>
    /// 构造目录树
    /// </summary>
    /// <param name="courseId"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<string> BuildTreeAsync(string courseId, string? id)
    {
        ThrowIfDisposed();
        if (courseId == null)
        {
            return "[]";
        }
        IList<Base_CourseCatalogue>? catalogues = await Store.FindByCourseIdAsync(courseId, id, CancellationToken).ConfigureAwait(false);
        if (catalogues?.Count > 0)
        {
            return BuildTree(catalogues, parentId: Guid.Empty.ToString());
        }
        return "[]";
    }

    private string BuildTree(IList<Base_CourseCatalogue> catalogues, string parentId)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append('[');
        foreach (var item in catalogues.Where(c => c.ParentId == parentId))
        {
            builder.Append('{');
            builder.AppendFormat("\"id\":\"{0}\"", item.Id);
            builder.AppendFormat(",\"name\":\"{0}\"", item.Content);
            builder.AppendFormat(",\"sortCode\":\"{0}\"", item.SortCode);
            if (catalogues.Any(c => c.ParentId == item.Id))
            {
                builder.AppendFormat(",\"children\":{0}", BuildTree(catalogues, item.Id));
            }
            builder.Append("},");
        }
        if (builder.ToString().Substring(builder.Length - 1) == ",")
        {
            builder.Remove(builder.Length - 1, 1);
        }
        builder.Append(']');
        return builder.ToString();
    }

    /// <summary>
    /// 设置排序代码
    /// </summary>
    /// <param name="catalogue"></param>
    /// <returns></returns>
    public async Task SetSortCodeAsync(Base_CourseCatalogue catalogue)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(catalogue);
        ArgumentException.ThrowIfNullOrWhiteSpace(catalogue.ParentId);
        var entry = await Store.GetMaxSortCodeAsync(catalogue.CourseId, parentId: catalogue.ParentId, CancellationToken).ConfigureAwait(false);
        if (entry == null)
        {
            if (catalogue.ParentId == Guid.Empty.ToString())
            {
                catalogue.SortCode = 1;
            }
            else
            {
                var parent = await FindByIdAsync(catalogue.ParentId).ConfigureAwait(false);
                if (parent == null)
                {
                    throw new InvalidOperationException($"Course catalogue ID {catalogue.ParentId} does not exist");
                }
                catalogue.SortCode = parent.SortCode * 10 + 1;
            }
        }
        else
        {
            catalogue.SortCode = entry.SortCode + 1;
        }
    }

    /// <summary>
    /// 导入多个课程目录
    /// </summary>
    /// <param name="models">课程目录集合 <see cref="IEnumerable<CourseCatalogueImportDto>"/></param>
    /// <returns></returns>
    public async Task<IdentityResult> ImportCourseCataloguesAsync(IEnumerable<CourseCatalogueImportDto> models)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(models);
        List<IdentityError>? errors = null;
        foreach (var model in models)
        {
            var result = await ImportCourseCatalogueAsync(model);
            if (!result.Succeeded)
            {
                errors ??= new List<IdentityError>();
                errors.AddRange(result.Errors);
            }
        }
        return errors?.Count > 0 ? IdentityResult.Failed(errors.ToArray()) : IdentityResult.Success;
    }

    private CourseManager GetCourseManager()
    {
        var manager = Service.GetRequiredService<CourseManager>();
        ArgumentNullException.ThrowIfNull(manager);
        return manager;
    }

    /// <summary>
    /// 导入单个模型
    /// </summary>
    /// <param name="model">课程目录导入信息 <see cref="CourseCatalogueImportDto"/></param>
    /// <returns></returns>
    public async Task<IdentityResult> ImportCourseCatalogueAsync(CourseCatalogueImportDto model)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(model);
        if (model.Course == null)
        {
            return IdentityResult.Failed(ErrorDescriber.InvalidError("课程"));
        }
        var course = await GetCourseManager().FindByNameAsync(model.Course).ConfigureAwait(false);
        if (course == null)
        {
            return IdentityResult.Failed(ErrorDescriber.InvalidError($"课程 {model.Course}"));
        }
        var config = new MapperConfiguration(cfg => cfg.CreateMap<CourseCatalogueImportDto, Base_CourseCatalogue>());
        var catalogue = config.CreateMapper().Map<Base_CourseCatalogue>(model);
        catalogue.CourseId = course.Id;
        if (model.ParentName != null)
        {
            var parents = await Store.FindListAsync(c => c.CourseId == course.Id && c.NormalizedContent == NormalizedData(model.ParentName), "SortCode", true, CancellationToken).ConfigureAwait(false);
            if (parents?.Count < 0)
            {
                return IdentityResult.Failed(ErrorDescriber.InvalidError($"上级内容 {model.ParentName}"));
            }
            catalogue.ParentId = parents!.First().Id;
        }
        else
        {
            catalogue.ParentId = Guid.Empty.ToString();
        }
        await SetSortCodeAsync(catalogue).ConfigureAwait(false);
        return await ImportAsync(catalogue).ConfigureAwait(false);
    }

    /// <summary>
    /// 分页查询
    /// </summary>
    /// <param name="parameter">分页查询参数 <see cref="DataTableParameter"/></param>
    /// <param name="courseId">课程主键</param>
    /// <param name="parentId">主级主键</param>
    /// <returns></returns>
    public async Task<string> FindPageAsync(DataTableParameter parameter, string courseId, string? parentId)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(parameter);
        var result = await Store.FindPageAsync(parameter, courseId, parentId, CancellationToken).ConfigureAwait(false);
        return JsonHelper.ConvertToJson(result);
    }
}