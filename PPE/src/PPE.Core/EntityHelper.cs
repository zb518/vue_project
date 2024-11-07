using AutoMapper;
using PPE.Model.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;

namespace PPE.Core;

public class EntityHelper
{

    public static string GetTableName(Type type)
    {
        return type.GetCustomAttribute<TableAttribute>(true)?.Name ?? type.Name;
    }

    public static string? GetColumnName(PropertyInfo property)
    {
        return property.GetCustomAttribute<ColumnAttribute>(true)?.Name;
    }


    public static void SetPropertyInfoValue<T>(T entity, PropertyInfo property, object value)
    {
        var type = property.PropertyType;
        var sourType = value.GetType();
        if (type.Name == "String")
        {
            property.SetValue(entity, value.ToString());
            return;
        }
        if (type.IsGenericType)
        {

            return;
        }
        if (type.IsEnum) //枚举类型值
        {
            if (type.GetFields().Any(x => x.Name.ToUpper() == value.ToString()!.ToUpper()))
            {
                Enum.TryParse(type, value.ToString(), true, out var ev);
                property.SetValue(entity, ev);
            }
            return;
        }
        property.SetValue(entity, value);
    }

    public static T ConvertToEnum<T>(Type type, object value)
    {
        return default(T)!;
    }

    public static List<ModelDetailInfo> GetModelDetails<T>()
    {
        var type = typeof(T);
        List<ModelDetailInfo> modelDetails = new List<ModelDetailInfo>();
        var i = 0;
        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty))
        {
            var dispAttr = property.GetCustomAttribute<DisplayAttribute>(true);
            var detail = new ModelDetailInfo
            {
                IsKey = IsPirmayKey(property),
                Name = property.Name,
                DisplayName = dispAttr?.Name,
                Order = dispAttr?.Order ?? i,
                GroupName = dispAttr?.GroupName,
                DbType = GetDbType(property.PropertyType),
                ColumnName = GetColumnName(property)
            };
            i++;
            modelDetails.Add(detail);
        }
        return modelDetails;
    }

    public static List<ModelDetailInfo> GetModelDetails<T>(T model)
    {
        var type = model!.GetType();
        List<ModelDetailInfo> modelDetails = new List<ModelDetailInfo>();
        var i = 0;
        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty))
        {
            var dispAttr = property.GetCustomAttribute<DisplayAttribute>(true);
            var detail = new ModelDetailInfo
            {
                IsKey = IsPirmayKey(property),
                Name = property.Name,
                DisplayName = dispAttr?.Name,
                Order = dispAttr?.Order ?? i,
                GroupName = dispAttr?.GroupName,
                DbType = GetDbType(property.PropertyType),
                ColumnName = GetColumnName(property),
                Value = property.GetValue(model)
            };
            i++;
            modelDetails.Add(detail);
        }
        return modelDetails;
    }

    public static DbType GetDbType(Type propertyType)
    {
        if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            var gt = propertyType.GetGenericTypeDefinition();
            return default;
        }
        else if (propertyType.IsEnum)
        {
            var et = propertyType.GetEnumUnderlyingType();
            return Enum.Parse<DbType>(et.Name, true);
        }
        return Enum.Parse<DbType>(propertyType.Name, true);
    }

    public static bool IsPirmayKey(PropertyInfo property)
    {
        return property.GetCustomAttribute<KeyAttribute>(true) != null;
    }
    /// <summary>
    /// 使用Automapper进行数据映射
    /// </summary>
    /// <param name="source">数据源实体 <see cref="S"/></param>
    /// <typeparam name="D">会的数据类型</typeparam>
    /// <typeparam name="S">源数据类型</typeparam>
    /// <returns></returns>
    public static D MapperMap<D, S>(S source)
    {
        var config = new MapperConfiguration(cfg => cfg.CreateMap<S, D>());
        return config.CreateMapper().Map<D>(source);
    }


    public static List<DataTablesColumnParameter> GetDataTablesColumns(Type type)
    {
        List<DataTablesColumnParameter> columns = new List<DataTablesColumnParameter>();
        var i = 0;
        foreach (var property in type.GetProperties())
        {
            var dispAttr = property.GetCustomAttribute<DisplayAttribute>(true);
            var dtColAttr = property.GetCustomAttribute<DataTablesColumnAttribute>(true);
            var paramter = new DataTablesColumnParameter
            {
                name = property.Name,
                title = dispAttr?.Name ?? property.Name,
                target = dispAttr?.Order ?? i,
                data = property.Name,
                DbType = GetDbType(property.PropertyType).ToString(),
                orderable = dtColAttr?.Orderable ?? false,
                searchable = dtColAttr?.Searchable ?? false,
                visible = dtColAttr?.Visible ?? false,
            };
            columns.Add(paramter);
            i++;
        }
        columns = columns.OrderBy(x => x.target).ToList();
        return columns;
    }

    public static string GetDbTypeName(Type propertyType)
    {
        var dbtype = GetDbType(propertyType);
        string name = Enum.GetName<DbType>(dbtype)!;
        return name;
    }
}
