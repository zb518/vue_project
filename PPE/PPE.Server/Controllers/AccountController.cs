using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using PPE.BLL;
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

    [HttpPost]
    public async Task<Results<Ok<AccessTokenResponse>, EmptyHttpResult, ProblemHttpResult>> Login([FromBody] LoginDto login, [FromQuery] bool? useCookies, [FromQuery] bool? useSessionCookies)
    {
        var useCookieScheme = (useCookies == true) || (useSessionCookies == true);
        var isPersistent = (useCookies == true) && (useSessionCookies != true);
        SignInManager.AuthenticationScheme = useCookieScheme ? IdentityConstants.ApplicationScheme : IdentityConstants.BearerScheme;

        var result = await SignInManager.PasswordSignInAsync(login.UserName, login.Password, isPersistent, lockoutOnFailure: true);

        if (result.RequiresTwoFactor)
        {
            if (!string.IsNullOrEmpty(login.TwoFactorCode))
            {
                result = await SignInManager.TwoFactorAuthenticatorSignInAsync(login.TwoFactorCode, isPersistent, rememberClient: isPersistent);
            }
            else if (!string.IsNullOrEmpty(login.TwoFactorRecoveryCode))
            {
                result = await SignInManager.TwoFactorRecoveryCodeSignInAsync(login.TwoFactorRecoveryCode);
            }
        }

        if (!result.Succeeded)
        {
            return TypedResults.Problem(result.ToString(), statusCode: StatusCodes.Status401Unauthorized);
        }

        // The signInManager already produced the needed response in the form of a cookie or bearer token.
        return TypedResults.Empty;
    }

    //[HttpPost]
    //public Task<bool> CheckUserNameAsync([FromBody] string userName)
    //{

    //}
}
