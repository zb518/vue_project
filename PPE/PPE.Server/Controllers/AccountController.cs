using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using PPE.BLL;
using PPE.Core;
using PPE.ModelDto;

namespace PPE.Server.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AccountController : ControllerBase
{
    public AccountController(ILogger<AccountController> logger, IServiceProvider service, UserManager userManager, SignInManager signInManager, SignLogManager logManager, IDistributedCache cache)
    {
        Logger = logger;
        Service = service;
        UserManager = userManager;
        SignInManager = signInManager;
        LogManager = logManager;
        Cache = cache;
    }

    public ILogger<AccountController> Logger { get; }
    public IServiceProvider Service { get; }
    public UserManager UserManager { get; }
    public SignInManager SignInManager { get; }
    public SignLogManager LogManager { get; }
    public IDistributedCache Cache { get; }

    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="useCookies"></param>
    /// <param name="useSessionCookies"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<Results<Ok<AccessTokenResponse>, EmptyHttpResult, ProblemHttpResult>> Login([FromBody] LoginDto dto, [FromQuery] bool? useCookies, [FromQuery] bool? useSessionCookies)
    {
        var useCookieScheme = (useCookies == true) || (useSessionCookies == true);
        var isPersistent = (useCookies == true) && (useSessionCookies != true);
        SignInManager.AuthenticationScheme = useCookieScheme ? IdentityConstants.ApplicationScheme : IdentityConstants.BearerScheme;

        var result = await SignInManager.PasswordSignInAsync(dto.UserName, dto.Password, isPersistent, lockoutOnFailure: true);

        if (result.RequiresTwoFactor)
        {
            if (!string.IsNullOrEmpty(dto.TwoFactorCode))
            {
                result = await SignInManager.TwoFactorAuthenticatorSignInAsync(dto.TwoFactorCode, isPersistent, rememberClient: isPersistent);
            }
            else if (!string.IsNullOrEmpty(dto.TwoFactorRecoveryCode))
            {
                result = await SignInManager.TwoFactorRecoveryCodeSignInAsync(dto.TwoFactorRecoveryCode);
            }
        }

        if (!result.Succeeded)
        {
            return TypedResults.Problem(result.ToString(), statusCode: StatusCodes.Status401Unauthorized);
        }

        // The signInManager already produced the needed response in the form of a cookie or bearer token.
        return TypedResults.Empty;
    }

    /// <summary>
    /// 生成验证码图片
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<FileResult> VerifyCode()
    {
        var code = ValidatorCodeHelper.CreateCode();
        await Cache.SetStringAsync(nameof(VerifyCode), code, new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(5),
            AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(5)
        });
        var image = ValidatorCodeHelper.CreatePng(code);
        return File(image, "image/png");
    }

    //[HttpPost]
    //public Task<bool> CheckUserNameAsync([FromBody] string userName)
    //{

    //}
}
