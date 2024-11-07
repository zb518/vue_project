using Microsoft.Extensions.Logging;
using PPE.Core;
using PPE.DataModel;
using PPE.IDAL;
using PPE.Model.Shared;

namespace PPE.BLL;

/// <summary>
/// 登录日志业务逻辑管理
/// </summary>
public class SignLogManager : IDisposable
{
    private bool _disposed;

    public IdentityFactory Identity { get; private set; }
    public OperationErrorDescriber ErrorDescriber { get; private set; }
    public ISignLogRepository Store { get; private set; }
    public ILogger Logger { get; private set; }
    public SignLogManager(ISignLogRepository repository, ILogger<SignLogManager> logger, IdentityFactory identityFactory, OperationErrorDescriber describer)
    {
        ArgumentNullException.ThrowIfNull(repository);
        Store = repository;
        Logger = logger;
        Identity = identityFactory;
        ErrorDescriber = describer ?? new OperationErrorDescriber();
    }
    private CancellationToken CancellationToken => CancellationToken.None;

    /// <summary>
    /// 创建登录日志
    /// </summary>
    /// <param name="userId">用户主键</param>
    /// <param name="userName">账号</param>
    /// <param name="realName">姓名</param>
    /// <param name="loginDate">登录日间</param>
    /// <param name="result">登录结果</param>
    /// <param name="description">说明</param>
    /// <returns></returns>
    public async Task CreateAsync(string? userId = null, string? userName = null, string? realName = null, DateTime? loginDate = null, bool result = false, string? description = null)
    {
        ThrowIfDisposed();
        var log = new Base_SignLog
        {
            UserId = userId,
            UserName = userName,
            Description = description,
            RealName = realName,
            LoginDate = loginDate ?? DateTime.Now,
            LoginIP = Identity.GetIPAddress(),
            LoginResult = result,
        };
        await Store.CreateAsync(log, CancellationToken).ConfigureAwait(false);
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(GetType().FullName);
        }
    }

    /// <summary>
    /// 更新注销时间
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="loginDate"></param>
    /// <returns></returns>
    public async Task LogoutAsync(string userId, DateTime? loginDate)
    {
        var log = (await Store.FindLastAsync(userId, Identity.GetIPAddress(), loginDate, CancellationToken).ConfigureAwait(false));
        if (log != null)
        {
            log.LogoutTime = DateTime.Now;
            await Store.UpdateAsync(log, CancellationToken).ConfigureAwait(false);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (disposing && !_disposed)
        {
            _disposed = true;
            Store.Dispose();
        }
    }
}
