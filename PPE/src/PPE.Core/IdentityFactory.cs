using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PPE.Model.Shared;
using System.Security.Claims;

namespace PPE.Core;

public class IdentityFactory
{
    private readonly IHttpContextAccessor _contextAccessor;

    public IdentityFactory(ILogger<IdentityFactory> logger, IOptions<IdentityOptions> optionAccessor, IHttpContextAccessor contextAccessor)
    {
        Logger = logger;
        Options = optionAccessor?.Value ?? new IdentityOptions();
        _contextAccessor = contextAccessor;
    }

    public IdentityOptions Options { get; }
    public ILogger<IdentityFactory> Logger { get; }

    /// <summary>
    /// The authentication scheme to sign in with. Defaults to <see cref="IdentityConstants.ApplicationScheme"/>.
    /// </summary>
    public string AuthenticationScheme { get; set; } = IdentityConstants.ApplicationScheme;


    private HttpContext? _context;
    // <summary>
    /// The <see cref="HttpContext"/> used.
    /// </summary>
    public HttpContext Context
    {
        get
        {
            var context = _context ?? _contextAccessor?.HttpContext;
            if (context == null)
            {
                throw new InvalidOperationException("HttpContext must not be null.");
            }
            return context;
        }
        set
        {
            _context = value;
        }
    }

    public virtual bool IsSignedIn() => IsSignedIn(Context.User);

    /// <summary>
    /// Returns true if the principal has an identity with the application cookie identity
    /// </summary>
    /// <param name="principal">The <see cref="ClaimsPrincipal"/> instance.</param>
    /// <returns>True if the user is logged in with identity.</returns>
    public virtual bool IsSignedIn(ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);
        return principal.Identities != null &&
            principal.Identities.Any(i => i.AuthenticationType == AuthenticationScheme);
    }

    /// <summary>
    /// 获取登录用户信息
    /// </summary>
    /// <returns></returns>
    public SignUser? GetSignUser() => GetSignUser(Context.User);

    /// <summary>
    /// 获取登录用户信息
    /// </summary>
    /// <param name="principal"><see cref="ClaimsPrincipal"/></param>
    /// <returns></returns>
    public SignUser? GetSignUser(ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);
        if (IsSignedIn(principal))
        {
            var options = Options.ClaimsIdentity;
            return new SignUser(id: principal.FindFirstValue(options.UserIdClaimType),
          userName: principal.FindFirstValue(options.UserNameClaimType),
           realName: principal.FindFirstValue("Identity.RealName"),
           email: principal.FindFirstValue(options.EmailClaimType),
           securityStamp: principal.FindFirstValue(options.SecurityStampClaimType),
           roles: principal.FindFirstValue(options.RoleClaimType)
            );
        }
        return default;
    }


    /// <summary>
    /// 获取客户端IP地址
    /// </summary>
    /// <returns></returns>
    public string GetIPAddress()
    {
        string ipAddr = Context.Request != null ? Context.Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? string.Empty : string.Empty;
        if (string.IsNullOrEmpty(ipAddr))
        {
            ipAddr = Context.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
        }
        return ipAddr;
    }

    public string GetArea()
    {
        return Context.Request.RouteValues["area"] as string ?? string.Empty;
    }

    public string GetPage()
    {
        return Context.Request.RouteValues["page"] as string ?? string.Empty;
    }
}