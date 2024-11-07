using PPE.DataModel;
using PPE.Model.Shared;
using PPE.ModelDto;

namespace PPE.IDAL;
/// <summary>
/// 课程目录数据访问操作
/// </summary>
public interface ICourseCatalogueRepository : IBaseRepository<Base_CourseCatalogue, LearnDbContext>
{
    /// <summary>
    /// 课程主键查询目录
    /// </summary>
    /// <param name="courseId">课程主键</param>
    /// <param name="parentId">父主键</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IList<Base_CourseCatalogue>?> FindByCourseIdAsync(string courseId, string? parentId, CancellationToken cancellationToken);

    /// <summary>
    /// 获取最大排序代码
    /// </summary>
    /// <param name="courseId">课程主键</param>
    /// <param name="parentId">父主键</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Base_CourseCatalogue?> GetMaxSortCodeAsync(string courseId, string parentId, CancellationToken cancellationToken);

    /// <summary>
    /// 分页查询
    /// </summary>
    /// <param name="parameter">分页查询参数 <see cref="DataTableParameter"/></param>
    /// <param name="courseId">课程主键</param>
    /// <param name="parentId">主级主键</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<DataTableResult<CourseCatalogueDetailDto>> FindPageAsync(DataTableParameter parameter, string courseId, string? parentId, CancellationToken cancellationToken);
}