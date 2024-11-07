using Microsoft.EntityFrameworkCore;
using PPE.DataModel;
using PPE.IDAL;
using PPE.Model.Shared;

namespace PPE.DAL.Common;
/// <summary>
/// 登录日志数据访问操作
/// </summary>
public class SignLogRepository : ISignLogRepository
{
    private bool _disposed;

    public SignLogRepository(CommonDbContext context, OperationErrorDescriber describer, IOperationLogRepository logRepository)
    {
        Context = context;
        ErrorDescriber = describer;
        LogStore = logRepository;
    }

    public CommonDbContext Context { get; }
    public OperationErrorDescriber ErrorDescriber { get; }
    public IOperationLogRepository LogStore { get; }

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

    public IQueryable<Base_SignLog> SignLogs => SignLogSet;
    public DbSet<Base_SignLog> SignLogSet => Context.SignLogs;

    /// <summary>
    /// 创建登录日志
    /// </summary>
    /// <param name="log"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task CreateAsync(Base_SignLog log, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(log);
        SignLogSet.Add(log);
        await SaveChanges(cancellationToken);
        await LogStore.WriteForCreateAsync(log, OperationLogType.Create, cancellationToken);
    }

    /// <summary>
    /// 查询最后登录日志
    /// </summary>
    /// <param name="userId">用户主键</param>
    /// <param name="ipAddress">登录IP地址</param>
    /// <param name="signInDate">登录时间</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Base_SignLog?> FindLastAsync(string userId, string ipAddress, DateTime? signInDate, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        ArgumentException.ThrowIfNullOrWhiteSpace(ipAddress);
        return SignLogs.Where(l => l.UserId == userId && l.LoginIP == ipAddress && l.LoginDate!.Value >= signInDate).OrderByDescending(l => l.LoginDate).FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// 更新登录日志
    /// </summary>
    /// <param name="log"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task UpdateAsync(Base_SignLog log, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(log);
        var owner = await SignLogSet.FindAsync(log.Id, cancellationToken);
        SignLogSet.Attach(log);
        SignLogSet.Update(log);
        try
        {
            await SaveChanges(cancellationToken);
            await LogStore.WriteForUpdateAsync(log, owner, OperationLogType.Update, cancellationToken);
        }
        catch (DbUpdateConcurrencyException) { }
    }
}