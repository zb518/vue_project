using PPE.DataModel;

namespace PPE.IDAL;
/// <summary>
/// 系统日志数据访问操作
/// </summary>
public interface ISystemLogRepository : IDisposable
{
    IQueryable<Base_Log> Logs { get; }
}