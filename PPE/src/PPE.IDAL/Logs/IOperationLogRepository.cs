using PPE.DataModel;
using PPE.Model.Shared;

namespace PPE.IDAL;
/// <summary>
/// 操作日志数据访问操作接口
/// </summary>
public interface IOperationLogRepository : IDisposable
{
    IQueryable<Base_OperationLog> OperationLogs { get; }

    /// <summary>
    /// 创建记录日志
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="logType"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task WriteForCreateAsync<T>(T entity, OperationLogType logType, CancellationToken cancellationToken);

    /// <summary>
    /// 更新记录日志
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="owner"></param>
    /// <param name="logType"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task WriteForUpdateAsync<T>(T entity, T owner, OperationLogType logType, CancellationToken cancellationToken);

    /// <summary>
    /// 移除记录日志
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task WriteForRemoveAsync<T>(T entity, CancellationToken cancellationToken);

    /// <summary>
    /// 操作日志分页查询
    /// </summary>
    /// <param name="parameter">查询参数，<see cref="DataTableParameter"/></param>
    /// <param name="users">用户用户</param>
    /// <param name="logTypes">操作类型，<see cref="OperationLogType"/></param>
    /// <param name="startDate">日志起始时间值 <see cref="DateTime"/></param>
    /// <param name="endDate">日志操作终止时间起始值 <see cref="DateTime"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns>分页查询结果 <see cref="DataTableResult"/></returns>
    Task<DataTableResult<Base_OperationLog>> FindPageAsync(DataTableParameter parameter, string[]? users, OperationLogType[]? logTypes, DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken);
}
