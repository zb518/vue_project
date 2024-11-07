using Microsoft.AspNetCore.Mvc;
using PPE.WebCore.Data;

namespace PPE.Server.Controllers;
/// <summary>
/// 
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
public class CommonController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<CommonController> _logger;

    public CommonController(ILogger<CommonController> logger, IServiceProvider service)
    {
        _logger = logger;
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

    [HttpPut]
    public async Task<IActionResult> Initialized()
    {
        try
        {
            await SeedData.Initialize(Service);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Ok(ex);
        }
    }
}

