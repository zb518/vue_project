using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PPE.Core;
using PPE.IDAL;
using PPE.Model.Shared;
using System.Linq.Expressions;

namespace PPE.DAL;
/// <summary>
/// 数据访问操作基类
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TContext"></typeparam>
public class BaseRepository<TEntity, TContext> : BaseRepository<TEntity, TContext, string>, IBaseRepository<TEntity, TContext>
where TEntity : BaseDataModel
where TContext : DbContext
{
    public BaseRepository(TContext context, OperationErrorDescriber describer, IdentityFactory identityFactory, IOperationLogRepository logRepository) : base(context, describer, identityFactory, logRepository)
    {
    }
}

/// <summary>
/// 数据访问操作基类
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TContext"></typeparam>
/// <typeparam name="TKey"></typeparam>
public class BaseRepository<TEntity, TContext, TKey> : IBaseRepository<TEntity, TContext, TKey>
    where TEntity : BaseDataModel<TKey>
    where TContext : DbContext
    where TKey : IEquatable<TKey>
{
    private bool _disposed = false;
    public BaseRepository(TContext context, OperationErrorDescriber describer, IdentityFactory identityFactory, IOperationLogRepository logRepository)
    {
        Context = context;
        ErrorDescriber = describer ?? new OperationErrorDescriber();
        Identity = identityFactory;
        LogStore = logRepository;
    }

    public IQueryable<TEntity> Entities => EntitySet;
    public DbSet<TEntity> EntitySet => Context.Set<TEntity>();

    public TContext Context { get; set; }
    public OperationErrorDescriber ErrorDescriber { get; set; }
    public IdentityFactory Identity { get; set; }
    public IOperationLogRepository LogStore { get; set; }

    public void Dispose()
    {
        _disposed = true;
    }

    protected virtual void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(GetType().Name);
        }
    }

    protected virtual bool AutoSaveChanges { get; set; } = true;

    protected virtual Task SaveChanges(CancellationToken cancellationToken) => AutoSaveChanges ? Context.SaveChangesAsync(cancellationToken) : Task.CompletedTask;


    /// <summary>
    /// 创建记录
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual async Task<IdentityResult> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(entity);
        entity.SetCreate(Identity.GetSignUser());
        EntitySet.Add(entity);
        await SaveChanges(cancellationToken);
        await LogStore.WriteForCreateAsync(entity, OperationLogType.Create, cancellationToken);
        return IdentityResult.Success;
    }

    /// <summary>
    /// 导入记录
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual async Task<IdentityResult> ImportAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(entity);
        entity.SetCreate(Identity.GetSignUser());
        EntitySet.Add(entity);
        await SaveChanges(cancellationToken);
        await LogStore.WriteForCreateAsync(entity, OperationLogType.Import, cancellationToken);
        return IdentityResult.Success;
    }

    /// <summary>
    /// 更新记录
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual async Task<IdentityResult> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(entity);
        var owner = await EntitySet.FindAsync(entity.Id, cancellationToken);
        if (owner == null)
        {
            throw new InvalidOperationException($"{entity.GetType().Name} Id not found.");
        }
        if (!string.Equals(owner.ConcurrencyStamp, entity.ConcurrencyStamp, StringComparison.OrdinalIgnoreCase))
        {
            return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
        }
        EntitySet.Attach(entity);
        entity.SetUpdate(Identity.GetSignUser());
        EntitySet.Update(entity);
        try
        {
            await SaveChanges(cancellationToken);
            await LogStore.WriteForUpdateAsync(entity, owner, OperationLogType.Update, cancellationToken);
            return IdentityResult.Success;
        }
        catch (DbUpdateConcurrencyException)
        {
            return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
        }
    }

    /// <summary>
    /// 删除记录
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual async Task<IdentityResult> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(entity);
        var owner = await EntitySet.FindAsync(entity.Id, cancellationToken);
        if (owner == null)
        {
            throw new InvalidOperationException($"{entity.GetType().Name} Id not found.");
        }
        if (owner.IsDeleted)
        {
            return IdentityResult.Failed(ErrorDescriber.AlreadyDeleteError(entity.ToString()!));
        }
        if (!string.Equals(owner.ConcurrencyStamp, entity.ConcurrencyStamp, StringComparison.OrdinalIgnoreCase))
        {
            return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
        }
        EntitySet.Attach(entity);
        entity.SetUpdate(Identity.GetSignUser());
        entity.IsDeleted = true;
        EntitySet.Update(entity);
        try
        {
            await SaveChanges(cancellationToken);
            await LogStore.WriteForUpdateAsync(entity, owner, OperationLogType.Delete, cancellationToken);
            return IdentityResult.Success;
        }
        catch (DbUpdateConcurrencyException)
        {
            return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
        }
    }

    /// <summary>
    /// 恢复删除记录
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual async Task<IdentityResult> RecoveryAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(entity);
        var owner = await EntitySet.FindAsync(entity.Id, cancellationToken);
        if (owner == null)
        {
            throw new InvalidOperationException($"{entity.GetType().Name} Id not found.");
        }
        if (!owner.IsDeleted)
        {
            return IdentityResult.Failed(ErrorDescriber.NotDeleteError(entity.ToString()!));
        }
        if (!string.Equals(owner.ConcurrencyStamp, entity.ConcurrencyStamp, StringComparison.OrdinalIgnoreCase))
        {
            return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
        }
        EntitySet.Attach(entity);
        entity.SetUpdate(Identity.GetSignUser());
        entity.IsDeleted = false;
        EntitySet.Update(entity);
        try
        {
            await SaveChanges(cancellationToken);
            await LogStore.WriteForUpdateAsync(entity, owner, OperationLogType.Recovery, cancellationToken);
            return IdentityResult.Success;
        }
        catch (DbUpdateConcurrencyException)
        {
            return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
        }
    }

    /// <summary>
    /// 移除记录
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual async Task<IdentityResult> RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(entity);
        if (!entity.IsDeleted)
        {
            return IdentityResult.Failed(ErrorDescriber.NotDeleteError(entity.ToString()!));
        }
        EntitySet.Remove(entity);
        try
        {
            await SaveChanges(cancellationToken);
            await LogStore.WriteForRemoveAsync(entity, cancellationToken);
            return IdentityResult.Success;
        }
        catch (DbUpdateConcurrencyException)
        {
            return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual async Task<TEntity?> FindAsync(object[] keys, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(keys);
        return await EntitySet.FindAsync(keys, cancellationToken);
    }

    /// <summary>
    /// 主键查询
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual async Task<TEntity?> FindByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(id);
        return await EntitySet.FindAsync(id, cancellationToken);
    }

    /// <summary>
    /// 条件表达式查询，返回单一记录
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>>? predicate, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        var query = Entities;
        if (predicate != null)
        {
            query = query.Where(predicate);
        }
        return query.FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// 条件表达式查询，返回单一记录
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <param name="order">排序字段</param>
    /// <param name="isDesc">排序方式，True：降序/False：升序</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>>? predicate, string? order, bool isDesc, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        var query = Entities;
        if (predicate != null)
        {
            query = query.Where(predicate);
        }
        order ??= "Id";
        query = ExpressionExtensions.OrderBy(query, order, isDesc);
        return query.FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// 条件表达式查询，返回记录集合
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual async Task<IList<TEntity>?> FindListAsync(Expression<Func<TEntity, bool>>? predicate, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        var query = Entities;
        if (predicate != null)
        {
            query = query.Where(predicate);
        }
        query = query.OrderBy(x => x.Id);
        return await query.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// 条件表达式查询，返回记录集合
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <param name="order">排序字段</param>
    /// <param name="isDesc">排序方式，True：降序/False：升序</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual async Task<IList<TEntity>?> FindListAsync(Expression<Func<TEntity, bool>>? predicate, string? order, bool isDesc, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        var query = Entities;
        if (predicate != null)
        {
            query = query.Where(predicate);
        }
        query = ExpressionExtensions.OrderBy(query, order ?? "Id", isDesc);
        return await query.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// 条件表达式查询记录是否存在
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? predicate, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        var query = Entities;
        if (predicate != null)
        {
            query = query.Where(predicate);
        }
        query = query.OrderBy(x => x.Id);
        return query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// 条件表达式查询记录数量
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        var query = Entities;
        if (predicate != null)
        {
            query = query.Where(predicate);
        }
        query = query.OrderBy(x => x.Id);
        return query.CountAsync(cancellationToken);
    }

    /// <summary>
    /// 条件表达式查询记录数量
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual Task<long> LongCountAsync(Expression<Func<TEntity, bool>>? predicate, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        var query = Entities;
        if (predicate != null)
        {
            query = query.Where(predicate);
        }
        query = query.OrderBy(x => x.Id);
        return query.LongCountAsync(cancellationToken);
    }
}