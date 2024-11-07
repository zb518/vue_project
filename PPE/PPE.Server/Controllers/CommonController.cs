using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PPE.Core;
using PPE.WebCore.Data;

namespace PPE.Server.Controllers;
/// <summary>
/// 
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
public class CommonController : BaseController
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };


    public CommonController(ILogger<CommonController> logger, IServiceProvider service) : base(logger)
    {
        Service = service;
    }

    public IServiceProvider Service { get; }

    [HttpGet]
    public IEnumerable<WeatherForecast> WeatherForecast()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    /// <summary>
    /// 初始化数据
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPut]
    public async Task<IActionResult> Initialized()
    {
        try
        {
            if (ConfigManager.Builder.Environment.IsDevelopment())
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                await SeedData.Initialize(Service);
                sw.Stop();
                var message = $"数据初始化成功，用时 {sw.ElapsedMilliseconds} 毫秒。";
                Logger.LogInformation(message);
                if (SeedData.Errors?.Count > 0)
                {
                    message = string.Join(",", SeedData.Errors.Select(e => e.Description));
                    Logger.LogWarning(message);
                }
                return Content(message);
            }
            return BadRequest();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
            return Ok(ex);
        }
    }
}

