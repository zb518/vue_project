using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PPE.DataModel;
using PPE.IDAL;
using PPE.Model.Shared;
using PPE.ModelDto;
using PPE.Utilities;
using System.Text;

namespace PPE.BLL;
/// <summary>
/// 专业业务逻辑管理
/// </summary>
public class MajorManager : BaseManager<Base_Major, LearnDbContext>
{
    public MajorManager(IServiceProvider service, OperationErrorDescriber describer, IMajorRepository repository, ILogger<MajorManager> logger) : base(service, describer, repository, logger)
    {
        Store = repository;
    }
    public new IMajorRepository Store { get; set; }

    /// <summary>
    /// 分页查询
    /// </summary>
    /// <param name="parameter">分页查询参数信息 <see cref="DataTableParameter"/></param>
    /// <returns></returns>
    public async Task<string> FindPageAsync(DataTableParameter parameter)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(parameter);
        DataTableResult<MajorDetailDto> result = await Store.FindPageAsync(parameter, CancellationToken).ConfigureAwait(false);
        return JsonHelper.ConvertToJson(result);
    }

    /// <summary>
    /// 代码查询专业
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public Task<Base_Major?> FindByCodeAsync(string code)
    {
        ThrowIfDisposed();
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        return Store.FirstOrDefaultAsync(c => c.NormalizedCode == NormalizedData(code), CancellationToken);
    }

    /// <summary>
    /// 名称查询专业
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Task<Base_Major?> FindByNameAsync(string name)
    {
        ThrowIfDisposed();
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        var normalizedName = NormalizedData(name);
        return Store.FirstOrDefaultAsync(c => c.NormalizedName == normalizedName, CancellationToken);
    }


    public override async Task<IdentityResult> ValidateAsync(Base_Major major)
    {
        var result = await base.ValidateAsync(major).ConfigureAwait(false);
        if (!result.Succeeded)
        {
            return result;
        }
        if (major.Code == null)
        {
            return IdentityResult.Failed(ErrorDescriber.InvalidError("专业代码"));
        }
        if (await Store.AnyAsync(m => !m.Id.Equals(major.Id) && m.NormalizedCode == major.NormalizedCode, CancellationToken).ConfigureAwait(false))
        {
            return IdentityResult.Failed(ErrorDescriber.DuplicateError($"专业代码 {major.Code}"));
        }
        if (major.Name == null)
        {
            return IdentityResult.Failed(ErrorDescriber.InvalidEmail("专业名称"));
        }

        if (await Store.AnyAsync(m => !m.Id.Equals(major.Id) && m.NormalizedName == major.NormalizedName, CancellationToken).ConfigureAwait(false))
        {
            return IdentityResult.Failed(ErrorDescriber.DuplicateError($"专业名称 {major.Name}"));
        }
        return IdentityResult.Success;
    }


    /// <summary>
    /// 导入专业
    /// </summary>
    /// <param name="dto">导入数据业务模型 <see cref="MajorImportDto"/></param>
    /// <returns></returns>
    public async Task<IdentityResult> ImportMajorAsync(MajorImportDto dto)
    {
        try
        {
            ThrowIfDisposed();
            ArgumentNullException.ThrowIfNull(dto);
            var config = new MapperConfiguration(cfg => cfg.CreateMap<MajorImportDto, Base_Major>());
            var major = config.CreateMapper().Map<Base_Major>(dto);
            return await ImportAsync(major).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, ex.Message);
            return IdentityResult.Failed(ErrorDescriber.DefaultError());
        }
    }

    public async Task<IdentityResult> ImportMajorAsync(IEnumerable<MajorImportDto> models)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(models);
        List<IdentityError>? errors = null;
        foreach (var model in models)
        {
            var result = await ImportMajorAsync(model).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                errors ??= new List<IdentityError>();
                errors.AddRange(result.Errors);
            }
        }
        return errors?.Count > 0 ? IdentityResult.Failed(errors.ToArray()) : IdentityResult.Success;
    }

    public async Task<string> BuildTreeAsync()
    {
        ThrowIfDisposed();
        var majors = await Store.FindListAsync(m => !m.IsDeleted, CancellationToken);
        if (majors?.Count > 0)
        {
            var tree = new StringBuilder();
            tree.Append("[");
            foreach (var major in majors.OrderBy(m => m.Code))
            {
                tree.Append("{");
                tree.AppendFormat("\"id\":\"{0}\"", major.Id);
                tree.AppendFormat(",\"code\":\"{0}\"", major.Code);
                tree.AppendFormat(",\"name\":\"{0}[{1}]\"", major.Name, major.Code);
                tree.Append("},");
            }
            tree = tree.Remove(tree.Length - 1, 1);
            tree.Append("]");
            return tree.ToString();
        }
        return "[]";
    }
}