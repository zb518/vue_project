using Microsoft.EntityFrameworkCore;
using PPE.Core;
using PPE.DataModel;
using PPE.IDAL;
using PPE.Model.Shared;
using PPE.ModelDto;

namespace PPE.DAL;
/// <summary>
/// 课程数据访问操作
/// </summary>
public class CourseRepository : BaseRepository<Base_Course, LearnDbContext>, ICourseRepository
{
    public CourseRepository(LearnDbContext context, OperationErrorDescriber describer, IdentityFactory identityFactory, IOperationLogRepository logRepository) : base(context, describer, identityFactory, logRepository)
    {
    }
    private DbSet<Base_Major> Majors => Context.Majors;
    private DbSet<Base_CourseCatalogue> CourseCatalogues => Context.CourseCatalogues;
    private DbSet<Base_CourseContent> CourseContents => Context.CourseContents;
    private DbSet<Base_CourseMajor> CourseMajors => Context.CourseMajors;

    /// <summary>
    /// 分页查询
    /// </summary>
    /// <param name="parameter">分页参数</param>
    /// <param name="majorId">专业主键</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<DataTableResult<CourseDetailDto>> FindPageAsync(DataTableParameter parameter, string? majorId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(parameter);
        var query = from course in Entities
                    join cm in CourseMajors on course.Id equals cm.CourseId
                    select new CourseDetailDto
                    {
                        Id = course.Id,
                        MajorId = cm.MajorId,
                        Code = course.Code,
                        NormalizedCode = course.NormalizedCode,
                        Name = course.Name,
                        NormalizedName = course.NormalizedName,
                        Url = course.Url,
                        NormalizedUrl = course.Url,
                        IsDeleted = course.IsDeleted,
                        Description = course.Description,
                    };
        if (majorId != null)
        {
            query = query.Where(c => c.MajorId == majorId);
        }
        var result = new DataTableResult<CourseDetailDto>(parameter.Draw, await query.LongCountAsync(cancellationToken), 0, null);
        if (parameter.Search?.Value != null)
        {
            var searchPrecicate = ExpressionExtensions.False<CourseDetailDto>();
            foreach (var column in parameter.Columns)
            {
                if (column.Data != null && column.Searchable)
                {
                    searchPrecicate = searchPrecicate.Or(ExpressionExtensions.Contains<CourseDetailDto>(column.Data, parameter.Search.Value));
                }
            }
            query = query.Where(searchPrecicate);
        }
        result.recordsFiltered = await query.LongCountAsync(cancellationToken);
        if (parameter.Order?.Count > 0)
        {
            var i = 0;
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
                    i++;
                }
            }
        }
        else
        {
            query = query.OrderBy(c => c.Name);
        }
        query = query.Skip(parameter.Start).Take(parameter.Length);
        result.data = await query.ToListAsync(cancellationToken);
        return result;
    }

    /// <summary>
    /// 代码查询专业
    /// </summary>
    /// <param name="code">专业代码</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Base_Major?> FindMajorByCodeAync(string code, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        return Majors.FirstOrDefaultAsync(m => m.Code == code, cancellationToken);
    }



    /// <summary>
    /// 课程是否已经在专业里
    /// </summary>
    /// <param name="course">课程信息 <see cref="Base_Course"/></param>
    /// <param name="major">专业信息 <see cref="Base_Major"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<bool> IsCourseInMajorAsync(Base_Course course, Base_Major major, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(course);
        ArgumentNullException.ThrowIfNull(major);
        return CourseMajors.AnyAsync(m => m.CourseId.Equals(course.Id) && m.MajorId.Equals(major.Id), cancellationToken);
    }

    /// <summary>
    /// 添加课程到专业
    /// </summary>
    /// <param name="course">课程信息 <see cref="Base_Course"/></param>
    /// <param name="major">专业信息 <see cref="Base_Major"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task AddCourseToMajorAsync(Base_Course course, Base_Major major, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(course);
        ArgumentNullException.ThrowIfNull(major);
        var courseMajor = new Base_CourseMajor
        {
            CourseId = course.Id,
            MajorId = major.Id
        };
        var signUser = Identity.GetSignUser();
        courseMajor.SetCreate(signUser);
        CourseMajors.Add(courseMajor);
        await SaveChanges(cancellationToken);
        await LogStore.WriteForCreateAsync(courseMajor, OperationLogType.Create, cancellationToken);
    }

    /// <summary>
    /// 从专业中移除课程
    /// </summary>
    /// <param name="course">课程信息 <see cref="Base_Course"/></param>
    /// <param name="major">专业信息 <see cref="Base_Major"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task RemoveCourseFromMajorAsync(Base_Course course, Base_Major major, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(course);
        ArgumentNullException.ThrowIfNull(major);
        var courseMajor = await CourseMajors.FindAsync([course.Id, major.Id], cancellationToken);
        if (courseMajor != null)
        {
            CourseMajors.Remove(courseMajor);
            await SaveChanges(cancellationToken);
            await LogStore.WriteForRemoveAsync(courseMajor, cancellationToken);
        }
    }
}