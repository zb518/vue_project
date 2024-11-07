using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using PPE.BLL;
using PPE.Core;
using PPE.DataModel;
using PPE.ModelDto;

namespace PPE.Server.Controllers;
/// <summary>
/// 账号 API
/// </summary>
[Route("api/[controller]/[action]")]
[ApiController]
public class AccountController : ControllerBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="service"></param>
    /// <param name="userManager"></param>
    /// <param name="signInManager"></param>
    /// <param name="logManager"></param>
    /// <param name="cache"></param>
    /// <param name="bearerTokenOptions"></param>
    /// <param name="timeProvider"></param>
    public AccountController(ILogger<AccountController> logger, IServiceProvider service, UserManager userManager, SignInManager signInManager, SignLogManager logManager, IDistributedCache cache, IOptionsMonitor<BearerTokenOptions> bearerTokenOptions, TimeProvider timeProvider)
    {
        Logger = logger;
        Service = service;
        UserManager = userManager;
        SignInManager = signInManager;
        LogManager = logManager;
        Cache = cache;
        BearerTokenOptions = bearerTokenOptions;
        TimeProvider = timeProvider;
    }

    public ILogger<AccountController> Logger { get; }
    public IServiceProvider Service { get; }
    public UserManager UserManager { get; }
    public SignInManager SignInManager { get; }
    public SignLogManager LogManager { get; }
    public IDistributedCache Cache { get; }
    public IOptionsMonitor<BearerTokenOptions> BearerTokenOptions { get; }
    public TimeProvider TimeProvider { get; }

    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<Results<Ok<AccessTokenResponse>, EmptyHttpResult, ProblemHttpResult>> Login([FromBody] LoginDto dto)
    {
        var result = await SignInAsync(dto);
        //await Cache.SetStringAsync("LoginResult", JsonHelper.ConvertToJson(result));
        return result;
    }

    // /// <summary>
    // /// 
    // /// </summary>
    // /// <param name="dto"></param>
    // /// <param name="useCookies"></param>
    // /// <param name="useSessionCookies"></param>
    // /// <returns></returns>
    // [HttpPost]
    // public async Task<Results<Ok<LoginResult>>> Login1([FromBody] LoginDto dto, [FromQuery] bool? useCookies, [FromQuery] bool? useSessionCookies)
    // {
    //     var loginResult = new LoginResult();
    //     var code = await Cache.GetStringAsync(nameof(VerifyCode));
    //     if (!string.Equals(code, dto.VerifyCode, StringComparison.OrdinalIgnoreCase))
    //     {
    //         loginResult.Message = "验证码不正确";
    //         return Ok(loginResult);
    //     }
    //     var user = await UserManager.FindByNameAsync(dto.UserName);
    //     if (user == null)
    //     {
    //         loginResult.Message = "登录账号错误";
    //         return Ok(loginResult);
    //     }
    //     var result = await SignInAsync(dto, useCookies, useSessionCookies);
    //     loginResult.AccessTokenResponse = result;
    //     return Ok(loginResult);
    // }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    private async Task<Results<Ok<AccessTokenResponse>, EmptyHttpResult, ProblemHttpResult>> SignInAsync(LoginDto dto)
    {
        SignInManager.AuthenticationScheme = IdentityConstants.BearerScheme;
        var user = await UserManager.FindByNameAsync(dto.UserName);
        if (user == null)
        {
            await LogManager.CreateAsync(userName: dto.UserName, result: false, description: "账号错误");
            return TypedResults.Problem("登录信息信息", statusCode: StatusCodes.Status401Unauthorized);
        }
        var result = await SignInManager.PasswordSignInAsync(user, dto.Password, false, lockoutOnFailure: true);
        if (result.IsNotAllowed)
        {
            await LogManager.CreateAsync(userId: user.Id, userName: user.UserName, realName: user.RealName, result: false, description: "密码错误");
            return TypedResults.Problem(result.ToString(), statusCode: StatusCodes.Status401Unauthorized);
        }
        if (result.IsLockedOut)
        {
            await LogManager.CreateAsync(userId: user.Id, userName: user.UserName, realName: user.RealName, result: false, description: $"密码错误 {UserManager.Options.Lockout.MaxFailedAccessAttempts} 次，账号锁定。");
            return TypedResults.Problem(result.ToString(), statusCode: StatusCodes.Status401Unauthorized);
        }
        if (result.RequiresTwoFactor)
        {
            if (!string.IsNullOrEmpty(dto.TwoFactorCode))
            {
                result = await SignInManager.TwoFactorAuthenticatorSignInAsync(dto.TwoFactorCode, false, rememberClient: false);
            }
            else if (!string.IsNullOrEmpty(dto.TwoFactorRecoveryCode))
            {
                result = await SignInManager.TwoFactorRecoveryCodeSignInAsync(dto.TwoFactorRecoveryCode);
            }
        }

        // if (!result.Succeeded)
        // {
        //     return TypedResults.Problem(result.ToString(), statusCode: StatusCodes.Status401Unauthorized);
        // }

        // The signInManager already produced the needed response in the form of a cookie or bearer token.
        if (result.Succeeded)
        {
            user.FirstAccess ??= DateTime.Now;
            user.PreviousAccess = user.LastAccess;
            user.LastAccess = DateTime.Now;
            user.AccessIP = UserManager.Identity.GetIPAddress();
            user.AccessCount++;
            await UserManager.UpdateLoginInfoAsync(user);
            await LogManager.CreateAsync(userId: user.Id, userName: user.UserName, realName: user.RealName, result: true, loginDate: user.LastAccess);
        }
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


    /// <summary>
    /// 刷新Token
    /// </summary>
    /// <param name="refreshRequest"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<Results<Ok<AccessTokenResponse>, UnauthorizedHttpResult, SignInHttpResult, ChallengeHttpResult>> Refresh([FromBody] RefreshRequest refreshRequest)
    {
        var refreshTokenProtector = BearerTokenOptions.Get(IdentityConstants.BearerScheme).RefreshTokenProtector;
        var refreshTicket = refreshTokenProtector.Unprotect(refreshRequest.RefreshToken)!;
        var token = BearerTokenOptions.CurrentValue.BearerTokenProtector.Protect(refreshTicket);
        // Reject the /refresh attempt with a 401 if the token expired or the security stamp validation fails
        if (refreshTicket?.Properties?.ExpiresUtc is not { } expiresUtc ||
            TimeProvider.GetUtcNow() >= expiresUtc ||
            await SignInManager.ValidateSecurityStampAsync(refreshTicket.Principal) is not Base_User user)

        {
            return TypedResults.Challenge();
        }

        var newPrincipal = await SignInManager.CreateUserPrincipalAsync(user);
        var result = TypedResults.SignIn(newPrincipal, authenticationScheme: IdentityConstants.BearerScheme);
        return result;
    }

    /// <summary>
    /// 验证账号
    /// </summary>
    /// <param name="vur"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<bool> ValidateUserNameAsync([FromBody] ValidateUserRequest vur)
    {
        if (string.IsNullOrWhiteSpace(vur?.UserName))
        {
            return false;
        }
        var user = await UserManager.FindByNameAsync(vur.UserName);
        return user != null;
    }

    /// <summary>
    /// 验证密码
    /// </summary>
    /// <param name="vpr"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<bool> ValidatePasswordAsync([FromBody] ValidatePasswordRequest vpr)
    {
        if (string.IsNullOrWhiteSpace(vpr?.UserName) || string.IsNullOrWhiteSpace(vpr?.Password))
        {
            return false;
        }
        var user = await UserManager.FindByNameAsync(vpr.UserName);
        return user != null;
    }

    /// <summary>
    /// 验证验证码
    /// </summary>
    /// <param name="vvr"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<bool> ValidateVerifyCodeAsync([FromBody] ValidateVerifyCodeRequtest vvr)
    {
        if (string.IsNullOrWhiteSpace(vvr?.VerifyCode))
        {
            return false;
        }
        var code = await Cache.GetStringAsync(nameof(VerifyCode));
        return string.Equals(code, vvr.VerifyCode, StringComparison.OrdinalIgnoreCase);
    }
}
