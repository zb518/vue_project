using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PPE.Server;
/// <summary>
/// 
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
[Authorize]
public class BaseController : ControllerBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    public BaseController(ILogger<BaseController> logger)
    {
        Logger = logger;
    }
    /// <summary>
    /// 
    /// </summary> <summary>
    /// 
    /// </summary>
    /// <value></value>
    public ILogger Logger { get; }
}
