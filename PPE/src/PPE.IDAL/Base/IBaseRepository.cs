using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PPE.Model.Shared;
using System.Linq.Expressions;

namespace PPE.IDAL;

/// <summary>
/// 数据访问操作基接口
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TContext"></typeparam>
public interface IBaseRepository<TEntity, TContext> : IBaseRepository<TEntity, TContext, string>
where TEntity : BaseDataModel
where TContext : DbContext
{

}

/// <summary>
/// 数据访问操作基接口
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TContext"></typeparam>
/// <typeparam name="TKey"></typeparam>
public interface IBaseRepository<TEntity, TContext, TKey> : IDisposable
    where TEntity : BaseDataModel<TKey>
    where TContext : DbContext
    where TKey : IEquatable<TKey>
{
    IQueryable<TEntity> Entities { get; }

    /// <summary>
    /// 创建记录
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IdentityResult> CreateAsync(TEntity entity, CancellationToken cancellationToken);

    /// <summary>
    /// 导入记录
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IdentityResult> ImportAsync(TEntity entity, CancellationToken cancellationToken);

    /// <summary>
    /// 更新记录
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IdentityResult> UpdateAsync(TEntity entity, CancellationToken cancellationToken);

    /// <summary>
    /// 删除记录
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IdentityResult> DeleteAsync(TEntity entity, CancellationToken cancellationToken);

    /// <summary>
    /// 恢复删除记录
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IdentityResult> RecoveryAsync(TEntity entity, CancellationToken cancellationToken);

    /// <summary>
    /// 移除记录
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IdentityResult> RemoveAsync(TEntity entity, CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TEntity?> FindAsync(object[] keys, CancellationToken cancellationToken);

    /// <summary>
    /// 主键查询
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TEntity?> FindByIdAsync(TKey id, CancellationToken cancellationToken);

    /// <summary>
    /// 条件表达式查询，返回单一记录
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>>? predicate, CancellationToken cancellationToken);

    /// <summary>
    /// 条件表达式查询，返回单一记录
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <param name="order">排序字段</param>
    /// <param name="isDesc">排序方式，True：降序/False：升序</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>>? predicate, string? order, bool isDesc, CancellationToken cancellationToken);

    /// <summary>
    /// 条件表达式查询，返回记录集合
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IList<TEntity>?> FindListAsync(Expression<Func<TEntity, bool>>? predicate, CancellationToken cancellationToken);

    /// <summary>
    /// 条件表达式查询，返回记录集合
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <param name="order">排序字段</param>
    /// <param name="isDesc">排序方式，True：降序/False：升序</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IList<TEntity>?> FindListAsync(Expression<Func<TEntity, bool>>? predicate, string? order, bool isDesc, CancellationToken cancellationToken);

    /// <summary>
    /// 条件表达式查询记录是否存在
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? predicate, CancellationToken cancellationToken);

    /// <summary>
    /// 条件表达式查询记录数量
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate, CancellationToken cancellationToken);

    /// <summary>
    /// 条件表达式查询记录数量
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<long> LongCountAsync(Expression<Func<TEntity, bool>>? predicate, CancellationToken cancellationToken);
}