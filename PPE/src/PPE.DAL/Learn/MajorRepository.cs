using Microsoft.EntityFrameworkCore;
using PPE.Core;
using PPE.DataModel;
using PPE.IDAL;
using PPE.Model.Shared;
using PPE.ModelDto;

namespace PPE.DAL;
/// <summary>
/// 专业数据访问操作
/// </summary>
public class MajorRepository : BaseRepository<Base_Major, LearnDbContext>, IMajorRepository
{
    public MajorRepository(LearnDbContext context, OperationErrorDescriber describer, IdentityFactory identityFactory, IOperationLogRepository logRepository) : base(context, describer, identityFactory, logRepository)
    {
    }
    private DbSet<Base_Course> Courses => Context.Courses;
    private DbSet<Base_CourseMajor> CourseMajors => Context.CourseMajors;

    /// <summary>
    /// 分页查询
    /// </summary>
    /// <param name="parameter">分页查询参数信息 <see cref="DataTableParameter"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<DataTableResult<MajorDetailDto>> FindPageAsync(DataTableParameter parameter, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        var query = Entities.AsNoTracking().
        Select(m => new MajorDetailDto
        {
            Id = m.Id,
            Code = m.Code,
            NormalizedCode = m.NormalizedCode,
            Name = m.Name,
            NormalizedName = m.NormalizedName,
            CurriculumGroup = m.CurriculumGroup,
            IsDeleted = m.IsDeleted,
            Description = m.Description,
            CreateDate = m.CreateDate,
            CreateRealName = m.CreateRealName,
            CreateUserName = m.CreateUserName,
            UpdateDate = m.UpdateDate,
            UpdateRealName = m.UpdateRealName,
            Level = m.Level,
            UpdateUserName = m.UpdateUserName
        });
        return await ExpressionExtensions.FindPageAsync(query, parameter);
    }

}