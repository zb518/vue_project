using PPE.DataModel;

namespace PPE.IDAL;
/// <summary>
/// 登录日志数据访问操作
/// </summary>
public interface ISignLogRepository : IDisposable
{
    IQueryable<Base_SignLog> SignLogs { get; }

    /// <summary>
    /// 创建登录日志
    /// </summary>
    /// <param name="log"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task CreateAsync(Base_SignLog log, CancellationToken cancellationToken);

    /// <summary>
    /// 查询最后登录日志
    /// </summary>
    /// <param name="userId">用户主键</param>
    /// <param name="ipAddress">登录IP地址</param>
    /// <param name="signInDate">登录时间</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Base_SignLog?> FindLastAsync(string userId, string ipAddress, DateTime? signInDate, CancellationToken cancellationToken);

    /// <summary>
    /// 更新登录日志
    /// </summary>
    /// <param name="log"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UpdateAsync(Base_SignLog log, CancellationToken cancellationToken);
}