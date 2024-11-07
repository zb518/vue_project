using PPE.DataModel;
using PPE.Model.Shared;

namespace PPE.IDAL;
/// <summary>
/// 系统日志数据访问操作
/// </summary>
public class SystemLogRepository : ISystemLogRepository
{
    private bool _disposed;

    public SystemLogRepository(SystemLogDbContext context, OperationErrorDescriber describer)
    {
        Context = context;
        ErrorDescriber = describer ?? new OperationErrorDescriber();
    }

    public SystemLogDbContext Context { get; }
    public OperationErrorDescriber ErrorDescriber { get; }

    public void Dispose()
    {
        _disposed = true;
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(GetType().Name);
        }
    }

    private bool AutoSaveChanges { get; set; } = true;

    private Task SaveChanges(CancellationToken cancellationToken) => AutoSaveChanges ? Context.SaveChangesAsync(cancellationToken) : Task.CompletedTask;
    public IQueryable<Base_Log> Logs => Context.Logs;


}
