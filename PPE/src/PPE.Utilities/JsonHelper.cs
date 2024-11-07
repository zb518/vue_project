using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace PPE.Utilities;
/// <summary>
/// JSON相关处理
/// </summary>
public class JsonHelper
{

    /// <summary>
    /// JSON序列化相关配置
    /// </summary>
    /// <value></value>
    public static JsonSerializerSettings Settings { get; set; } = new JsonSerializerSettings
    {
        ContractResolver = new DefaultContractResolver(),
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        DateFormatString = "yyyy-MM-dd HH:mm:ss"
    };

    /// <summary>
    /// JToken 序列化为JSON字符串
    /// </summary>
    /// <param name="token">token 值 <see cref="JToken"/></param>
    /// <returns></returns>
    public static string JTokenToJson(JToken token)
    {
        return JTokenToJson(token, Settings);
    }

    /// <summary>
    /// JToken 序列化为JSON字符串
    /// </summary>
    /// <param name="token">token 值 <see cref="JToken"/></param>
    /// <param name="settings">JSON序列化配置 <see cref="JsonSerializerSettings"/></param>
    /// <returns></returns>
    public static string JTokenToJson(JToken token, JsonSerializerSettings settings)
    {

        return JsonConvert.SerializeObject(token, settings);
    }

    /// <summary>
    /// Ojbect 转换为JSON字符串
    /// </summary>
    /// <param name="value">数据源 <see cref="object"/></param>
    /// <returns></returns>
    public static string ConvertToJson(object value)
    {
        return ConvertToJson(value, Settings);
    }

    /// <summary>
    /// Ojbect 转换为JSON字符串
    /// </summary>
    /// <param name="value">数据源 <see cref="object"/></param>
    /// <param name="settings">JSON序列化配置 <see cref="JsonSerializerSettings"/></param>
    /// <returns></returns>
    public static string ConvertToJson(object value, JsonSerializerSettings settings)
    {
        return JsonConvert.SerializeObject(value, settings);
    }

    /// <summary>
    /// JSON字符串序列化为实体对象
    /// </summary>
    /// <param name="value">JSON字符串</param>
    /// <typeparam name="T">实体对象类型</typeparam>
    /// <returns></returns>
    public static T? ConvertToModel<T>(string value)
    {
        return ConvertToModel<T>(value, Settings);
    }

    /// <summary>
    /// JSON字符串序列化为实体对象
    /// </summary>
    /// <param name="value">JSON字符串</param>
    /// <param name="settings"></param>
    /// <typeparam name="T">JSON序列化配置 <see cref="JsonSerializerSettings"/></typeparam>
    /// <returns></returns>
    public static T? ConvertToModel<T>(string value, JsonSerializerSettings settings)
    {
        return JsonConvert.DeserializeObject<T>(value, settings);
    }
}