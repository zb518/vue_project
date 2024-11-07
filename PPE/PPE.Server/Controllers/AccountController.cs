using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using PPE.BLL;
using PPE.Core;
using PPE.DataModel;
using PPE.Model.Shared;
using PPE.ModelDto;

namespace PPE.Server.Controllers;
/// <summary>
/// 账号 API
/// </summary>
[Route("api/[controller]/[action]")]
[ApiController]
public class AccountController : BaseController
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
    public AccountController(ILogger<AccountController> logger, IServiceProvider service, UserManager userManager, SignInManager signInManager, SignLogManager logManager, IDistributedCache cache, IOptionsMonitor<BearerTokenOptions> bearerTokenOptions, TimeProvider timeProvider) : base(logger)
    {
        Service = service;
        UserManager = userManager;
        SignInManager = signInManager;
        LogManager = logManager;
        Cache = cache;
        BearerTokenOptions = bearerTokenOptions;
        TimeProvider = timeProvider;
    }


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
    [AllowAnonymous]
    public async Task<Results<Ok<AccessTokenResponse>, EmptyHttpResult, ProblemHttpResult>> Login([FromBody] LoginDto dto)
    {
        var result = await SignInAsync(dto);
        //await Cache.SetStringAsync("LoginResult", JsonHelper.ConvertToJson(result));
        return result;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    private async Task<Results<Ok<AccessTokenResponse>, EmptyHttpResult, ProblemHttpResult>> SignInAsync(LoginDto dto)
    {
        var code = await Cache.GetStringAsync(nameof(VerifyCode));
        if (!string.Equals(code, dto.VerifyCode, StringComparison.OrdinalIgnoreCase))
        {
            return TypedResults.Problem("验证码错误", statusCode: StatusCodes.Status200OK);
        }
        SignInManager.AuthenticationScheme = IdentityConstants.BearerScheme;
        var user = await UserManager.FindByNameAsync(dto.UserName);
        if (user == null)
        {
            await LogManager.CreateAsync(userName: dto.UserName, result: false, description: "账号错误");
            return TypedResults.Problem("账号错误", statusCode: StatusCodes.Status200OK);
        }
        var result = await SignInManager.PasswordSignInAsync(user, dto.Password, false, lockoutOnFailure: true);
        if (result.IsNotAllowed)
        {
            await LogManager.CreateAsync(userId: user.Id, userName: user.UserName, realName: user.RealName, result: false, description: "密码错误");
            return TypedResults.Problem("密码错误", statusCode: StatusCodes.Status200OK);
        }
        if (result.IsLockedOut)
        {
            await LogManager.CreateAsync(userId: user.Id, userName: user.UserName, realName: user.RealName, result: false, description: $"密码错误 {UserManager.Options.Lockout.MaxFailedAccessAttempts} 次，账号锁定。");
            return TypedResults.Problem($"密码错误 {UserManager.Options.Lockout.MaxFailedAccessAttempts} 次，账号锁定，请稍候。", statusCode: StatusCodes.Status200OK);
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
            else
            {
                await LogManager.CreateAsync(userId: user.Id, userName: user.UserName, realName: user.RealName, result: false, description: "需要两种验证");
                return TypedResults.Problem("需要两种验证", statusCode: StatusCodes.Status200OK);
            }
        }
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
    [AllowAnonymous]
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
    [AllowAnonymous]
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
    /// 获取登录用户信息
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public IActionResult SignInUser()
    {
        var signinUser = UserManager.Identity.GetSignUser(User);
        return Ok(signinUser);
    }


}
