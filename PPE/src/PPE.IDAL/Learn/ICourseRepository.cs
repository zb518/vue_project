using PPE.DataModel;
using PPE.Model.Shared;
using PPE.ModelDto;

namespace PPE.IDAL;
/// <summary>
/// 课程数据访问操作
/// </summary>
public interface ICourseRepository : IBaseRepository<Base_Course, LearnDbContext>
{
    /// <summary>
    /// 分页查询
    /// </summary>
    /// <param name="parameter">分页参数</param>
    /// <param name="majorId">专业主键</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<DataTableResult<CourseDetailDto>> FindPageAsync(DataTableParameter parameter, string? majorId, CancellationToken cancellationToken);


    /// <summary>
    /// 代码查询专业
    /// </summary>
    /// <param name="normalizedCode">专业代码</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Base_Major?> FindMajorByCodeAync(string normalizedCode, CancellationToken cancellationToken);

    /// <summary>
    /// 课程是否已经在专业里
    /// </summary>
    /// <param name="course">课程信息 <see cref="Base_Course"/></param>
    /// <param name="major">专业信息 <see cref="Base_Major"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> IsCourseInMajorAsync(Base_Course course, Base_Major major, CancellationToken cancellationToken);

    /// <summary>
    /// 添加课程到专业
    /// </summary>
    /// <param name="course">课程信息 <see cref="Base_Course"/></param>
    /// <param name="major">专业信息 <see cref="Base_Major"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task AddCourseToMajorAsync(Base_Course course, Base_Major major, CancellationToken cancellationToken);

    /// <summary>
    /// 从专业中移除课程
    /// </summary>
    /// <param name="course">课程信息 <see cref="Base_Course"/></param>
    /// <param name="major">专业信息 <see cref="Base_Major"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RemoveCourseFromMajorAsync(Base_Course course, Base_Major major, CancellationToken cancellationToken);
}