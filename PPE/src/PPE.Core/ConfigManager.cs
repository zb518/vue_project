using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PPE.Utilities;

namespace PPE.Core;
/// <summary>
/// 配置文件管理
/// </summary>
public class ConfigManager
{
    public static WebApplicationBuilder Builder { get; set; } = default!;

    private static readonly object _locker = new object();
    private static ConfigManager? _instance = null;
    protected string JsonConfigFile { get; set; } = "appsettings";
    /// <summary>
    /// 
    /// </summary>
    /// <param name="configFile"></param>
    public ConfigManager(string configFile)
    {
        Configuration = Builder.Configuration;
        Environment = Builder.Environment;
        JsonConfigFile = configFile ?? "appsettings";
        JsonConfigFile += ".json";
        if (!File.Exists(JsonConfigFile))
        {
            File.WriteAllText(JsonConfigFile, "{}");
        }
        if (!string.Equals(JsonConfigFile, "appsettings.json", StringComparison.OrdinalIgnoreCase))
        {
            Builder.Configuration.AddJsonFile(JsonConfigFile, optional: true, reloadOnChange: true)
                   .AddJsonFile($"{JsonConfigFile.Substring(0, JsonConfigFile.LastIndexOf("."))}.{Builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
        }
        Configuration = Builder.Configuration;
    }


    public ConfigurationManager Configuration { get; }
    public IWebHostEnvironment Environment { get; }

    public static ConfigManager Instance(string configFile = "appsettings")
    {
        lock (_locker)
        {
            if (_instance == null)
            {
                _instance = new ConfigManager(configFile);
            }
        }
        return _instance;
    }



    /// <summary>
    /// 获取指定配置字符串值
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public string GetConfigString(string key)
    {
        return Configuration.GetSection(key).Value ?? string.Empty;
    }

    /// <summary>
    /// 读取配置
    /// </summary>
    /// <param name="key">关键字</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T? Read<T>(string key)
    {
        var section = Configuration.GetSection(key);
        if (section != null)
        {
            using (var sr = new StreamReader(JsonConfigFile))
            {
                using (var jtr = new JsonTextReader(sr))
                {
                    var token = JToken.ReadFrom(jtr);
                    var value = token[key];
                    if (value == null)
                    {
                        return default;
                    }
                    var jsonStr = JsonHelper.JTokenToJson(value);
                    var result = JsonHelper.ConvertToModel<T>(jsonStr);
                    return result;
                }
            }
        }
        return default;
    }

    /// <summary>
    /// 更新配置
    /// </summary>
    /// <param name="key">关键字</param>
    /// <param name="value">配置值</param>
    /// <typeparam name="T"></typeparam>
    public void Update<T>(string key, T value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        JToken? token = null;
        using (var sr = new StreamReader(JsonConfigFile))
        {
            using (var jtr = new JsonTextReader(sr))
            {
                token = JToken.ReadFrom(jtr);
                token[key] = value != null ? JToken.FromObject(value) : null;
            }
        }
        Save(token);
    }

    /// <summary>
    /// 保存配置
    /// </summary>
    /// <param name="token"></param>
    private void Save(JToken token)
    {
        try
        {
            using (var sw = new StreamWriter(JsonConfigFile))
            {
                using (var jtw = new JsonTextWriter(sw))
                {
                    jtw.Formatting = Formatting.Indented;
                    jtw.Indentation = 4;
                    jtw.IndentChar = ' ';
                    token.WriteTo(jtw);
                }
            }
        }
        catch (Exception ex)
        {
            var logger = Builder.Services.BuildServiceProvider().CreateScope().ServiceProvider.GetRequiredService<ILogger<ConfigManager>>();
            logger.LogError(ex, ex.Message);
        }
    }

    /// <summary>
    /// 获取连接字符串
    /// </summary>
    /// <param name="name">连接字符串名称</param>
    /// <returns></returns>
    public string GetConnectionString(string name)
    {
        return Configuration.GetConnectionString(name) ?? throw new InvalidOperationException($"Connection string '{name} not found.");
    }
}