using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PPE.Core;
using PPE.DataModel;
using PPE.Model.Shared;
using System.Reflection;

namespace PPE.IDAL.Common;
/// <summary>
/// 操作日志数据访问操作类
/// </summary>
public class OperationLogRepository : IOperationLogRepository
{
    private bool _disposed;

    public OperationLogRepository(CommonDbContext context, OperationErrorDescriber describer, IdentityFactory identityFactory, ILogger<OperationLogRepository> logger)
    {
        Context = context;
        ErrorDescriber = describer ?? new OperationErrorDescriber();
        Identity = identityFactory;
        Logger = logger;
    }


    public CommonDbContext Context { get; }
    public OperationErrorDescriber ErrorDescriber { get; }
    public IdentityFactory Identity { get; }
    public ILogger<OperationLogRepository> Logger { get; }

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

    public IQueryable<Base_OperationLog> OperationLogs => OperationLogSet;
    private DbSet<Base_OperationLog> OperationLogSet => Context.OperationLogs;
    private DbSet<Base_OperationLogDetail> OperationLogDetails => Context.OperationLogDetails;

    private Base_OperationLog CreateLog(Type type, OperationLogType logType)
    {
        var signInUser = Identity.GetSignUser();
        var log = new Base_OperationLog
        {
            EntityName = type.Name,
            TableName = EntityHelper.GetTableName(type),
            OperateIP = Identity.GetIPAddress(),
            OperationLogType = logType,
            OperateDate = DateTime.Now,
            UserId = signInUser?.Id,
            UserName = signInUser?.UserName,
            RealName = signInUser?.RealName,
        };
        return log;
    }

    /// <summary>
    /// 创建记录日志
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="logType"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public async Task WriteForCreateAsync<T>(T entity, OperationLogType logType, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(entity);
        var type = entity.GetType();
        var log = CreateLog(type, logType);
        OperationLogSet.Add(log);
        try
        {
            await SaveChanges(cancellationToken);
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.GetProperty))
            {
                var value = property.GetValue(entity);
                if (value != null)
                {
                    var detail = new Base_OperationLogDetail
                    {
                        FieldName = property.Name,
                        ColumnName = EntityHelper.GetColumnName(property) ?? property.Name,
                        DbType = EntityHelper.GetDbType(property.PropertyType),
                        OperateLogId = log.Id,
                        NewValue = value.ToString()
                    };
                    OperationLogDetails.Add(detail);
                }
            }
            await SaveChanges(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            Logger.LogError(ex, ex.Message);
        }
    }

    /// <summary>
    /// 更新记录日志
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="owner"></param>
    /// <param name="logType"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public async Task WriteForUpdateAsync<T>(T entity, T owner, OperationLogType logType, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(entity);
        ArgumentNullException.ThrowIfNull(owner);
        var type = entity.GetType();
        var log = CreateLog(type, logType);
        OperationLogSet.Add(log);
        try
        {
            await SaveChanges(cancellationToken);
            var o_type = owner.GetType();
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.GetProperty))
            {
                var value = property.GetValue(entity);
                var ov = o_type.GetProperty(property.Name)!.GetValue(owner);
                if (value == null && ov == null) continue;
                var detail = new Base_OperationLogDetail
                {
                    FieldName = property.Name,
                    ColumnName = EntityHelper.GetColumnName(property) ?? property.Name,
                    DbType = EntityHelper.GetDbType(property.PropertyType),
                    OperateLogId = log.Id,
                    NewValue = value?.ToString(),
                    OldValue = ov?.ToString(),
                };
                OperationLogDetails.Add(detail);

            }
            await SaveChanges(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            Logger.LogError(ex, ex.Message);
        }
    }

    /// <summary>
    /// 移除记录日志
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public async Task WriteForRemoveAsync<T>(T entity, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(entity);
        var type = entity.GetType();
        var log = CreateLog(type, OperationLogType.Remove);
        OperationLogSet.Add(log);
        try
        {
            await SaveChanges(cancellationToken);
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.GetProperty))
            {
                var value = property.GetValue(entity);
                if (value != null)
                {
                    var detail = new Base_OperationLogDetail
                    {
                        FieldName = property.Name,
                        ColumnName = EntityHelper.GetColumnName(property) ?? property.Name,
                        DbType = EntityHelper.GetDbType(property.PropertyType),
                        OperateLogId = log.Id,
                        OldValue = value.ToString()
                    };
                    OperationLogDetails.Add(detail);
                }
            }
            await SaveChanges(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            Logger.LogError(ex, ex.Message);
        }
    }


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
    public async Task<DataTableResult<Base_OperationLog>> FindPageAsync(DataTableParameter parameter, string[]? users, OperationLogType[]? logTypes, DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(parameter);
        var query = OperationLogs;
        var result = new DataTableResult<Base_OperationLog>(parameter.Draw, await query.LongCountAsync(cancellationToken), 0, null);
        if (users?.Length > 0)
        {
            var userPredicate = ExpressionExtensions.False<Base_OperationLog>();
            foreach (var user in users)
            {
                userPredicate = userPredicate.Or(o => o.UserId == user);
            }
            query = query.Where(userPredicate);
        }
        if (logTypes?.Length > 0)
        {
            var otPredicate = ExpressionExtensions.False<Base_OperationLog>();
            foreach (var logType in logTypes)
            {
                otPredicate = otPredicate.Or(o => o.OperationLogType == logType);
            }
            query = query.Where(otPredicate);
        }
        if (parameter.Search?.Value != null)
        {
            var searchPredicate = ExpressionExtensions.False<Base_OperationLog>();
            foreach (var column in parameter.Columns)
            {
                if (column.Data != null && column.Searchable)
                {
                    searchPredicate = searchPredicate.Or(ExpressionExtensions.Contains<Base_OperationLog>(column.Data, parameter.Search.Value));
                }
            }
            query = query.Where(searchPredicate);
        }
        startDate ??= DateTime.Now.AddMonths(-1);
        endDate ??= DateTime.Now;
        query = query.Where(l => l.OperateDate!.Value >= startDate.Value && l.OperateDate <= endDate.Value);
        result.recordsFiltered = await query.LongCountAsync(cancellationToken);

        if (parameter.Order?.Count > 0)
        {
            int i = 0;
            foreach (var order in parameter.Order)
            {
                var column = parameter.Columns[order.Column];
                if (column.Data != null && column.Orderable)
                {
                    if (i == 0)
                    {
                        query = ExpressionExtensions.OrderBy(query, column.Data, order.Dir == OrderDirection.Desc ? true : false);
                    }
                    else
                    {
                        query = ExpressionExtensions.OrderByThen(query, column.Data, order.Dir == OrderDirection.Desc ? true : false);
                    }
                    i++;
                }
            }
        }
        else
        {
            query = query.OrderByDescending(o => o.Id);
        }
        query = query.Skip(parameter.Start).Take(parameter.Length);
        result.data = await query.ToListAsync(cancellationToken);
        return result;
    }
}
