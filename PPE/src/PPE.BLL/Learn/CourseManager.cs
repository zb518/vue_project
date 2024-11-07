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
/// 课程业务逻辑管理
/// </summary>
public class CourseManager : BaseManager<Base_Course, LearnDbContext>
{
    public CourseManager(IServiceProvider service, OperationErrorDescriber describer, ICourseRepository repository, ILogger<CourseManager> logger) : base(service, describer, repository, logger)
    {
        Store = repository;
    }
    public new ICourseRepository Store { get; set; }

    public override async Task<IdentityResult> ValidateAsync(Base_Course course)
    {
        var result = await base.ValidateAsync(course).ConfigureAwait(false);
        if (!result.Succeeded)
        {
            return result;
        }
        if (course.Name == null)
        {
            return IdentityResult.Failed(ErrorDescriber.InvalidError("课程名称"));
        }
        if (course.Code == null)
        {
            return IdentityResult.Failed(ErrorDescriber.InvalidError("课程代码"));
        }
        if (await Store.AnyAsync(c => !c.Id.Equals(course.Id) && c.NormalizedName == course.NormalizedName, CancellationToken).ConfigureAwait(false))
        {
            return IdentityResult.Failed(ErrorDescriber.DuplicateError($"课程名称 {course.Name}"));
        }
        if (await Store.AnyAsync(c => !c.Id.Equals(course.Id) && c.NormalizedCode == course.NormalizedCode, CancellationToken).ConfigureAwait(false))
        {
            return IdentityResult.Failed(ErrorDescriber.DuplicateError($"课程代码 {course.Code}"));
        }
        return IdentityResult.Success;
    }

    public async Task<IdentityResult> ImportCoursesAsync(IEnumerable<CourseImportDto> models)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(models);
        List<IdentityError>? errors = null;
        foreach (var model in models)
        {
            var result = await ImportCourseAsync(model);
            if (!result.Succeeded)
            {
                errors ??= new List<IdentityError>();
                errors.AddRange(result.Errors);
            }
        }
        return errors?.Count > 0 ? IdentityResult.Failed(errors.ToArray()) : IdentityResult.Success;
    }

    public async Task<IdentityResult> ImportCourseAsync(CourseImportDto dto)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(dto);
        var config = new MapperConfiguration(cfg => cfg.CreateMap<CourseImportDto, Base_Course>());
        var course = config.CreateMapper().Map<Base_Course>(dto);
        var result = await ValidateAsync(course).ConfigureAwait(false);
        if (!result.Succeeded)
        {
            return result;
        }
        return await ImportAsync(course).ConfigureAwait(false);
    }


    /// <summary>
    /// 代码查询课程
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public Task<Base_Course?> FindByCodeAsync(string code)
    {
        ThrowIfDisposed();
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        return Store.FirstOrDefaultAsync(c => c.NormalizedCode == NormalizedData(code), CancellationToken);
    }

    /// <summary>
    /// 名称查询课程
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Task<Base_Course?> FindByNameAsync(string name)
    {
        ThrowIfDisposed();
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        var normalizedName = NormalizedData(name);
        return Store.FirstOrDefaultAsync(c => c.NormalizedName == normalizedName, CancellationToken);
    }

    /// <summary>
    /// 添加课程到专业
    /// </summary>
    /// <param name="courseCode">课程代码</param>
    /// <param name="majorCode">专业代码</param>
    /// <returns></returns>
    public async Task<IdentityResult> AddCourseToMajorAsync(string courseCode, string majorCode)
    {
        ThrowIfDisposed();
        ArgumentException.ThrowIfNullOrWhiteSpace(courseCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(majorCode);
        var course = await FindByCodeAsync(NormalizedData(courseCode)).ConfigureAwait(false);
        if (course == null)
        {
            throw new InvalidOperationException($"课程代码 {courseCode} 不存在。");
        }
        var major = await Store.FindMajorByCodeAync(NormalizedData(majorCode), CancellationToken).ConfigureAwait(false);
        if (major == null)
        {
            throw new InvalidOperationException($"专业代码 {majorCode} 不存在。");
        }
        if (await Store.IsCourseInMajorAsync(course, major, CancellationToken).ConfigureAwait(false))
        {
            return IdentityResult.Failed(ErrorDescriber.DuplicateError($"课程 {courseCode} 已经在 专业 {majorCode} 中。"));
        }
        await Store.AddCourseToMajorAsync(course, major, CancellationToken).ConfigureAwait(false);
        return IdentityResult.Success;
    }

    /// <summary>
    /// 添加课程到多个专业
    /// </summary>
    /// <param name="courseCode">课程代码</param>
    /// <param name="majorCodes">专业代码集合</param>
    /// <returns></returns>
    public async Task<IdentityResult> AddCourseToMajorsAsync(string courseCode, IEnumerable<string> majorCodes)
    {
        ThrowIfDisposed();
        ArgumentException.ThrowIfNullOrEmpty(courseCode);
        ArgumentNullException.ThrowIfNull(majorCodes);
        List<IdentityError>? errors = null;
        foreach (var major in majorCodes)
        {
            var result = await AddCourseToMajorAsync(courseCode, major).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                errors ??= new List<IdentityError>();
                errors.AddRange(result.Errors);
            }
        }
        return errors?.Count > 0 ? IdentityResult.Failed(errors.ToArray()) : IdentityResult.Success;
    }

    /// <summary>
    /// 从专业中移除课程
    /// </summary>
    /// <param name="courseCode">课程代码</param>
    /// <param name="majorCode">专业代码</param>
    /// <returns></returns>
    public async Task<IdentityResult> AddRemoveCourseFromeMajorAsync(string courseCode, string majorCode)
    {
        ThrowIfDisposed();
        ArgumentException.ThrowIfNullOrWhiteSpace(courseCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(majorCode);
        var course = await FindByCodeAsync(NormalizedData(courseCode)).ConfigureAwait(false);
        if (course == null)
        {
            throw new InvalidOperationException($"课程代码 {courseCode} 不存在。");
        }
        var major = await Store.FindMajorByCodeAync(NormalizedData(majorCode), CancellationToken).ConfigureAwait(false);
        if (major == null)
        {
            throw new InvalidOperationException($"专业代码 {majorCode} 不存在。");
        }
        if (await Store.IsCourseInMajorAsync(course, major, CancellationToken).ConfigureAwait(false))
        {
            return IdentityResult.Failed(ErrorDescriber.DuplicateError($"课程 {courseCode} 已经在 专业 {majorCode} 中。"));
        }
        await Store.AddCourseToMajorAsync(course, major, CancellationToken).ConfigureAwait(false);
        return IdentityResult.Success;
    }

    /// <summary>
    /// 从多处专业中移除课程
    /// </summary>
    /// <param name="courseCode">课程代码</param>
    /// <param name="majorCodes">专业代码集合</param>
    /// <returns></returns>
    public async Task<IdentityResult> AddRemvoeCourseFromMajorsAsync(string courseCode, IEnumerable<string> majorCodes)
    {
        ThrowIfDisposed();
        ArgumentException.ThrowIfNullOrEmpty(courseCode);
        ArgumentNullException.ThrowIfNull(majorCodes);
        List<IdentityError>? errors = null;
        foreach (var major in majorCodes)
        {
            var result = await AddCourseToMajorAsync(courseCode, major).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                errors ??= new List<IdentityError>();
                errors.AddRange(result.Errors);
            }
        }
        return errors?.Count > 0 ? IdentityResult.Failed(errors.ToArray()) : IdentityResult.Success;
    }

    /// <summary>
    /// 导入课程专业关系
    /// </summary>
    /// <param name="courseMajors">课程专业关系导入业务模型 <see cref="CourseMajorDto"/></param>
    /// <returns></returns>
    public async Task<IdentityResult> ImportCoureMajorsAsync(IEnumerable<CourseMajorDto> courseMajors)
    {
        ThrowIfDisposed();
        List<IdentityError>? errors = null;
        foreach (var item in courseMajors)
        {
            if (item.CourseCode == null)
            {
                errors ??= new List<IdentityError>();
                errors.Add(ErrorDescriber.InvalidError($"课程代码"));
            }
            else if (item.MajorCode == null)
            {
                errors ??= new List<IdentityError>();
                errors.Add(ErrorDescriber.InvalidError($"专业代码"));
            }
            else
            {
                var result = await AddCourseToMajorAsync(item.CourseCode, item.MajorCode).ConfigureAwait(false);
                if (!result.Succeeded)
                {
                    errors ??= new List<IdentityError>();
                    errors.AddRange(result.Errors);
                }
            }
        }
        return errors?.Count > 0 ? IdentityResult.Failed(errors.ToArray()) : IdentityResult.Success;
    }

    /// <summary>
    /// 分页查询
    /// </summary>
    /// <param name="parameter">分页参数</param>
    /// <param name="majorId">专业主键</param>
    /// <returns></returns>
    public async Task<string> FindPageAsync(DataTableParameter parameter, string? majorId)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(parameter);
        var result = await Store.FindPageAsync(parameter, majorId, CancellationToken);
        return JsonHelper.ConvertToJson(result);
    }
}