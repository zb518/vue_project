using PPE.DataModel;
using PPE.Model.Shared;
using PPE.ModelDto;

namespace PPE.IDAL;
/// <summary>
/// 专业数据访问操作
/// </summary>
public interface IMajorRepository : IBaseRepository<Base_Major, LearnDbContext>
{
    /// <summary>
    /// 分页查询
    /// </summary>
    /// <param name="parameter">分页查询参数信息 <see cref="DataTableParameter"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<DataTableResult<MajorDetailDto>> FindPageAsync(DataTableParameter parameter, CancellationToken cancellationToken);
}