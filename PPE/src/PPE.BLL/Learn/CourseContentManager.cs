using Microsoft.Extensions.Logging;
using PPE.DataModel;
using PPE.IDAL;
using PPE.Model.Shared;

namespace PPE.BLL;
/// <summary>
/// 课程内容业务逻辑答管理
/// </summary>
public class CourseContentManager : BaseManager<Base_CourseContent, LearnDbContext>
{
    public CourseContentManager(IServiceProvider service, OperationErrorDescriber describer, ICourseContentRepository repository, ILogger<CourseContentManager> logger) : base(service, describer, repository, logger)
    {
        Store = repository;
    }
    public new ICourseContentRepository Store { get; set; }

    public Task<Base_CourseContent?> FindByCourseCatalogueIdAsync(string ccId)
    {
        ThrowIfDisposed();
        ArgumentException.ThrowIfNullOrWhiteSpace(ccId);
        return Store.FirstOrDefaultAsync(c => c.CourseCatalogueId == ccId, CancellationToken);
    }
}