using Microsoft.EntityFrameworkCore;
using PPE.Core;
using PPE.DataModel;
using PPE.IDAL;
using PPE.Model.Shared;
using PPE.ModelDto;

namespace PPE.DAL;
/// <summary>
/// 课程目录数据访问操作
/// </summary>
public class CourseCatalogueRepository : BaseRepository<Base_CourseCatalogue, LearnDbContext>, ICourseCatalogueRepository
{
    public CourseCatalogueRepository(LearnDbContext context, OperationErrorDescriber describer, IdentityFactory identityFactory, IOperationLogRepository logRepository) : base(context, describer, identityFactory, logRepository)
    {
    }

    private DbSet<Base_Course> Courses => Context.Courses;
    private DbSet<Base_CourseContent> CourseContents => Context.CourseContents;

    /// <summary>
    /// 课程主键查询目录
    /// </summary>
    /// <param name="courseId">课程主键</param>
    /// <param name="parentId">父主键</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IList<Base_CourseCatalogue>?> FindByCourseIdAsync(string courseId, string? parentId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentException.ThrowIfNullOrWhiteSpace(courseId);
        var query = Entities.Where(c => c.CourseId == courseId);
        if (parentId != null)
        {
            query = query.Where(c => c.ParentId == parentId);
        }
        query = query.OrderBy(c => c.SortCode);
        return await query.ToListAsync(cancellationToken);
    }



    /// <summary>
    /// 获取最大排序代码
    /// </summary>
    /// <param name="courseId">课程主键</param>
    /// <param name="parentId">父主键</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Base_CourseCatalogue?> GetMaxSortCodeAsync(string courseId, string parentId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentException.ThrowIfNullOrWhiteSpace(parentId);
        var entry = await Entities.Where(c => c.ParentId == parentId && c.CourseId == courseId).OrderByDescending(c => c.SortCode).FirstOrDefaultAsync(cancellationToken);
        return entry;
    }



    /// <summary>
    /// 分页查询
    /// </summary>
    /// <param name="parameter">分页查询参数 <see cref="DataTableParameter"/></param>
    /// <param name="courseId">课程主键</param>
    /// <param name="parentId">主级主键</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<DataTableResult<CourseCatalogueDetailDto>> FindPageAsync(DataTableParameter parameter, string courseId, string? parentId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(parameter);
        var query = from cc in Entities
                    select new CourseCatalogueDetailDto
                    {
                        Id = cc.Id,
                        ParentId = cc.ParentId,
                        CourseId = cc.CourseId,
                        Content = cc.Content,
                        NormalizedContent = cc.NormalizedContent,
                        IsDeleted = cc.IsDeleted,
                        Description = cc.Description,
                        CreateUserName = cc.CreateUserName,
                        CreateRealName = cc.CreateRealName,
                        CreateDate = cc.CreateDate,
                        UpdateUserName = cc.UpdateUserName,
                        UpdateRealName = cc.UpdateRealName,
                        UpdateDate = cc.UpdateDate,
                        SortCode = cc.SortCode,
                    };
        query = query.Where(c => c.CourseId == courseId);
        if (parentId != null)
        {
            query = query.Where(c => c.Id == parentId || c.ParentId == parentId);
        }
        return ExpressionExtensions.FindPageAsync(query, parameter, null, cancellationToken);
    }
}