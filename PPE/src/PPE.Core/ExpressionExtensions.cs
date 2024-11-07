using Microsoft.EntityFrameworkCore;
using PPE.Model.Shared;
using System.Linq.Expressions;

namespace PPE.Core;

/// <summary>
/// Expression extension
/// </summary>
public static class ExpressionExtensions
{
    /// <summary>
    /// 排序
    /// </summary>
    /// <param name="query"></param>
    /// <param name="orderName"></param>
    /// <param name="isDesc"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, string orderName, bool isDesc)
    {
        var type = query.ElementType;
        var parameterExp = Expression.Parameter(type, "p");
        var orderBy = isDesc ? "OrderByDescending" : "OrderBy";
        var memberExp = Expression.PropertyOrField(parameterExp, orderName);
        if (memberExp == null)
        {
            return query;
        }
        var accessExp = Expression.MakeMemberAccess(parameterExp, memberExp.Member);
        var orderExp = Expression.Lambda(accessExp, parameterExp);
        var resultExpt = Expression.Call(typeof(Queryable), orderBy, [type, memberExp.Type], query.Expression, Expression.Quote(orderExp));
        return query.Provider.CreateQuery<T>(resultExpt);
    }

    /// <summary>
    /// 多字段排序时，先调用OrderBy，再调用本方法
    /// </summary>
    /// <param name="query"></param>
    /// <param name="orderName"></param>
    /// <param name="isDesc"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IQueryable<T> OrderByThen<T>(this IQueryable<T> query, string orderName, bool isDesc)
    {
        var type = query.ElementType;
        var parameterExp = Expression.Parameter(type, "p");
        var orderBy = isDesc ? "ThenByDescending" : "ThenBy";
        var memberExp = Expression.PropertyOrField(parameterExp, orderName);
        if (memberExp == null)
        {
            return query;
        }
        var accessExp = Expression.MakeMemberAccess(parameterExp, memberExp.Member);
        var orderExp = Expression.Lambda(accessExp, parameterExp);
        var resultExpt = Expression.Call(typeof(Queryable), orderBy, [type, memberExp.Type], query.Expression, Expression.Quote(orderExp));
        return query.Provider.CreateQuery<T>(resultExpt);
    }


    /// <summary>
    /// 机关函数应用True时：单个AND有效，多个AND有效；单个OR无效，多个OR无效；混应时写在AND后的OR有效。即，设置为True时所有or语句应该放在and语句之后，否则无效
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Expression<Func<T, bool>> True<T>() => r => true;
    /// <summary>
    /// 机关函数应用False时：单个AND无效，多个AND无效；单个OR有效，多个OR有效；混应时写在OR后面的AND有效。 即，设置为False时所有or语句应该放在and语句之前，否则无效
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Expression<Func<T, bool>> False<T>() => r => false;

    /// <summary>
    /// 模糊查询
    /// </summary>
    /// <param name="propertyName">属性或字段名称</param>
    /// <param name="value">要查询的值</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Expression<Func<T, bool>> Contains<T>(string propertyName, object value)
    {
        var parameterExp = Expression.Parameter(typeof(T));
        var memberExp = Expression.PropertyOrField(parameterExp, propertyName);
        if (memberExp == null)
        {
            throw new InvalidOperationException($"属性或字段 {propertyName} 不存在");
        }
        if (memberExp.Type != typeof(string))
        {
            throw new InvalidOperationException($"数据类型必须为 String 类型");
        }
        var methodInfo = memberExp.Type.GetMethod("Contains", [memberExp.Type]);
        var constantExp = Expression.Constant(value, memberExp.Type);
        return Expression.Lambda<Func<T, bool>>(Expression.Call(memberExp, methodInfo!, constantExp), parameterExp);
    }

    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
    {
        var invokeExp = Expression.Invoke(right, left.Parameters.Cast<Expression>());
        return Expression.Lambda<Func<T, bool>>(Expression.Or(left.Body, invokeExp), left.Parameters);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
    {
        var invokeExp = Expression.Invoke(right, left.Parameters.Cast<Expression>());
        return Expression.Lambda<Func<T, bool>>(Expression.Or(left.Body, invokeExp), left.Parameters);
    }



    /// <summary>
    /// 通用分页查询
    /// </summary>
    /// <param name="query">查询数据源</param>
    /// <param name="parameter">分页查询参数 <see cref="DataTableParameter"/></param>
    /// <param name="preCondition">前置查询表达式，用于计算总数 <see cref="Expression<Func<T, bool>>"/></param>
    /// <param name="condition">其它查询条件表达式 <see cref="Expression<Func<T, bool>>"/></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T">类型占位符</typeparam>
    /// <returns>返回查询结果参数 <see cref="DataTableResult<>"/></returns>
    public static async Task<DataTableResult<T>> FindPageWithPreAsync<T>(this IQueryable<T> query, DataTableParameter parameter, Expression<Func<T, bool>>? preCondition = null, Expression<Func<T, bool>>? condition = null, CancellationToken cancellationToken = default) where T : new()
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(parameter);
        var result = new DataTableResult<T>();
        result.draw = parameter.Draw;
        if (preCondition != null)
        {
            query = query.Where(preCondition);
        }
        result.recordsTotal = await query.LongCountAsync(cancellationToken);
        if (parameter.Search?.Value != null)
        {
            var searchPredicate = False<T>();
            foreach (var column in parameter.Columns)
            {
                if (column.Data != null && column.Searchable)
                {
                    searchPredicate = searchPredicate.Or<T>(Contains<T>(column.Data, parameter.Search.Value));
                }
            }
            query = query.Where(searchPredicate);
        }
        if (condition != null)
        {
            query = query.Where(condition);
        }

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
                        query = OrderBy<T>(query, column.Data, order.Dir == OrderDirection.Desc ? true : false);
                    }
                    else
                    {
                        query = OrderByThen<T>(query, column.Data, order.Dir == OrderDirection.Desc ? true : false);
                    }
                    ++i;
                }
            }
        }
        else
        {
            query = OrderBy(query, GetDefaultOrderField(typeof(T)), false);
        }
        query = query.Skip(parameter.Start).Take(parameter.Length);
        result.data = await query.ToListAsync(cancellationToken);
        return result;
    }

    /// <summary>
    /// 分页查询扩展API
    /// </summary>
    /// <param name="query">查询数据源</param>
    /// <param name="parameter">分页查询参数 <see cref="DataTableParameter"/></param>
    /// <param name="predicate">其它查询条件表达式 <see cref="Expression<Func<T, bool>>"/></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns>返回查询结果参数 <see cref="DataTableResult<>"/></returns>
    public static Task<DataTableResult<T>> FindPageAsync<T>(this IQueryable<T> query, DataTableParameter parameter, Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default) where T : new()
    {
        return FindPageWithPreAsync(query, parameter: parameter, preCondition: null, condition: predicate, cancellationToken);
        // ArgumentNullException.ThrowIfNull(query);
        // ArgumentNullException.ThrowIfNull(parameter);
        // var result = new DataTableResult<T>();
        // result.draw = parameter.Draw;
        // result.recordsTotal = await query.LongCountAsync(cancellationToken);
        // if (parameter.Search?.Value != null)
        // {
        //     var searchPredicate = False<T>();
        //     foreach (var column in parameter.Columns)
        //     {
        //         if (column.Data != null && column.Searchable)
        //         {
        //             searchPredicate = searchPredicate.Or<T>(Contains<T>(column.Data, parameter.Search.Value));
        //         }
        //     }
        //     query = query.Where(searchPredicate);
        // }
        // if (predicate != null)
        // {
        //     query = query.Where(predicate);
        // }

        // result.recordsFiltered = await query.LongCountAsync(cancellationToken);

        // if (parameter.Order?.Count > 0)
        // {
        //     int i = 0;
        //     foreach (var order in parameter.Order)
        //     {
        //         var column = parameter.Columns[order.Column];
        //         if (column.Data != null && column.Orderable)
        //         {
        //             if (i == 0)
        //             {
        //                 query = OrderBy<T>(query, column.Data, order.Dir == OrderDirection.Desc ? true : false);
        //             }
        //             else
        //             {
        //                 query = OrderByThen<T>(query, column.Data, order.Dir == OrderDirection.Desc ? true : false);
        //             }
        //             ++i;
        //         }
        //     }
        // }
        // else
        // {
        //     query = OrderBy(query, GetDefaultOrderField(typeof(T)), false);
        // }
        // query = query.Skip(parameter.Start).Take(parameter.Length);
        // result.data = await query.ToListAsync(cancellationToken);
        // return result;
    }

    /// <summary>
    /// 获取排序字段
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static string GetDefaultOrderField(Type type)
    {
        var property = type.GetProperty("Id");
        if (property == null)
        {
            property = type.GetProperties().FirstOrDefault();
        }
        return property!.Name;
    }
}
