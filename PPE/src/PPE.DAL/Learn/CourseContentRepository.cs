using Microsoft.EntityFrameworkCore;
using PPE.Core;
using PPE.DataModel;
using PPE.IDAL;
using PPE.Model.Shared;

namespace PPE.DAL;
/// <summary>
/// 课程内容数据访问操作
/// </summary>
public class CourseContentRepository : BaseRepository<Base_CourseContent, LearnDbContext>, ICourseContentRepository
{
    public CourseContentRepository(LearnDbContext context, OperationErrorDescriber describer, IdentityFactory identityFactory, IOperationLogRepository logRepository) : base(context, describer, identityFactory, logRepository)
    {
    }

    private DbSet<Base_Course> Courses => Context.Courses;
    private DbSet<Base_CourseCatalogue> CourseCatalogues => Context.CourseCatalogues;

}