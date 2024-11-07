using Microsoft.Extensions.Logging;
using PPE.DataModel;
using PPE.IDAL;
using PPE.Model.Shared;
using PPE.Utilities;

namespace PPE.BLL.Common;
/// <summary>
/// 操作日志业务逻辑管理
/// </summary>
public class OperationLogManager : IDisposable
{
    private readonly ILogger<OperationLogManager> _logger;

    public IOperationLogRepository Store { get; private set; }
    public OperationErrorDescriber ErrorDescriber { get; private set; }

    private bool _disposed;
    public OperationLogManager(ILogger<OperationLogManager> logger, IOperationLogRepository repository, OperationErrorDescriber? describer = null)
    {
        _logger = logger;
        Store = repository;
        ErrorDescriber = describer ?? new OperationErrorDescriber();
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
        }
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(GetType().Name);
        }
    }

    private CancellationToken CancellationToken => CancellationToken.None;


    public IQueryable<Base_OperationLog> operationLogs => Store.OperationLogs;

    /// <summary>
    /// 操作日志分页查询
    /// </summary>
    /// <param name="parameter">查询参数，<see cref="DataTableParameter"/></param>
    /// <param name="users">用户用户</param>
    /// <param name="logTypes">操作类型，<see cref="OperationLogType"/></param>
    /// <param name="startDate">日志起始时间值 <see cref="DateTime"/></param>
    /// <param name="endDate">日志操作终止时间起始值 <see cref="DateTime"/></param>
    /// <returns>分页查询结果,JSON 格式</returns>
    public async Task<string> FindPageAsync(DataTableParameter parameter, string[]? users, OperationLogType[]? logTypes, DateTime? startDate, DateTime? endDate)
    {
        ThrowIfDisposed();
        var result = await Store.FindPageAsync(parameter, users, logTypes, startDate, endDate, CancellationToken).ConfigureAwait(false);
        return JsonHelper.ConvertToJson(result);
    }
}

