using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PPE.IDAL;
using PPE.Model.Shared;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace PPE.BLL;

public abstract class BaseManager<TEntity, TContext> : BaseManager<TEntity, TContext, string>
where TEntity : BaseDataModel
where TContext : DbContext
{
    public BaseManager(IServiceProvider service, OperationErrorDescriber describer, IBaseRepository<TEntity, TContext> repository, ILogger<BaseManager<TEntity, TContext>> logger) : base(service, describer, repository, logger)
    {
        Store = repository;
    }
    public new IBaseRepository<TEntity, TContext> Store { get; set; }
}

public abstract class BaseManager<TEntity, TContext, TKey> : IDisposable
    where TEntity : BaseDataModel<TKey>
    where TContext : DbContext
    where TKey : IEquatable<TKey>
{
    protected bool _disposed;
    public BaseManager(IServiceProvider service, OperationErrorDescriber describer, IBaseRepository<TEntity, TContext, TKey> repository, ILogger<BaseManager<TEntity, TContext, TKey>> logger)
    {
        Service = service;
        ErrorDescriber = describer;
        Store = repository;
        Logger = logger;
    }

    public ILogger Logger { get; set; }
    public IServiceProvider Service { get; }
    public OperationErrorDescriber ErrorDescriber { get; }
    public IBaseRepository<TEntity, TContext, TKey> Store { get; }

    public IQueryable<TEntity> Entities => Store.Entities;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing && !_disposed)
        {
            _disposed = true;
            Store.Dispose();
        }
    }

    protected virtual void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(GetType().Name);
        }
    }

    public CancellationToken CancellationToken => CancellationToken.None;

    [return: NotNullIfNotNull("data")]
    public virtual string? NormalizedData(string? data)
    {
        return data?.ToUpper();
    }

    /// <summary>
    /// 主键查询
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public virtual Task<TEntity?> FindByIdAsync(TKey id)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(id);
        return Store.FindByIdAsync(id, CancellationToken);
    }

    /// <summary>
    /// 条件表达式查询记录是否存在
    /// </summary>
    /// <param name="predicate">条件表达式 <see cref="Expression<Func<TEntity, bool>>"/></param>
    /// <returns></returns>
    public Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? predicate)
    {
        ThrowIfDisposed();
        return Store.AnyAsync(predicate, CancellationToken);
    }

    /// <summary>
    /// 校验
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public virtual Task<IdentityResult> ValidateAsync(TEntity entity)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(entity);
        var type = entity.GetType();
        string prefix = "Normalized";
        var properties = type.GetProperties()
        .Where(x => x.Name.StartsWith(prefix)).ToList();
        foreach (var property in properties)
        {
            var name = property.Name.Substring(prefix.Length);
            var value = type.GetProperty(name)?.GetValue(entity);
            if (value != null)
            {
                property.SetValue(entity, NormalizedData(value.ToString()));
            }
        }
        return Task.FromResult(IdentityResult.Success);
    }

    /// <summary>
    /// 创建记录
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public async Task<IdentityResult> CreateAsync(TEntity entity)
    {
        ThrowIfDisposed();
        var result = await ValidateAsync(entity).ConfigureAwait(false);
        if (!result.Succeeded)
        {
            return result;
        }
        return await Store.CreateAsync(entity, CancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// 导入记录
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public async Task<IdentityResult> ImportAsync(TEntity entity)
    {
        ThrowIfDisposed();
        var result = await ValidateAsync(entity).ConfigureAwait(false);
        if (!result.Succeeded)
        {
            return result;
        }
        return await Store.ImportAsync(entity, CancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// 更新记录
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public async Task<IdentityResult> UpdateAsync(TEntity entity)
    {
        ThrowIfDisposed();
        var result = await ValidateAsync(entity).ConfigureAwait(false);
        if (!result.Succeeded)
        {
            return result;
        }
        return await Store.UpdateAsync(entity, CancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// 删除记录
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public async Task<IdentityResult> DeleteAsync(TEntity entity)
    {
        ThrowIfDisposed();
        var result = await ValidateAsync(entity).ConfigureAwait(false);
        if (!result.Succeeded)
        {
            return result;
        }
        return await Store.DeleteAsync(entity, CancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// 恢复删除记录
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public async Task<IdentityResult> RecoveryAsync(TEntity entity)
    {
        ThrowIfDisposed();
        var result = await ValidateAsync(entity).ConfigureAwait(false);
        if (!result.Succeeded)
        {
            return result;
        }
        return await Store.RecoveryAsync(entity, CancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// 移除记录
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public async Task<IdentityResult> RemoveAsync(TEntity entity)
    {
        ThrowIfDisposed();
        var result = await ValidateAsync(entity).ConfigureAwait(false);
        if (!result.Succeeded)
        {
            return result;
        }
        return await Store.RemoveAsync(entity, CancellationToken).ConfigureAwait(false);
    }


    public Task<IList<TEntity>?> FindListAsync(Expression<Func<TEntity, bool>>? predicate)
    {
        ThrowIfDisposed();
        return Store.FindListAsync(predicate, CancellationToken);
    }

    public Task<IList<TEntity>?> FindListAsync(Expression<Func<TEntity, bool>>? predicate, string order, bool isDesc)
    {
        ThrowIfDisposed();
        return Store.FindListAsync(predicate, order, isDesc, CancellationToken);
    }
}