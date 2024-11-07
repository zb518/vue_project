using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PPE.Model.Shared;
using System.Security.Claims;

namespace PPE.BLL;

/// <summary>
/// 权限业务逻辑管理
/// </summary>
public class PermissionManager : AuthorizationHandler<PermissionRequirement>
{
    public PermissionManager(ILogger<PermissionManager> logger, UserManager userManager, RoleManager roleManager)
    {
        Logger = logger;
        UserManager = userManager;
        RoleManager = roleManager;
    }

    public ILogger<PermissionManager> Logger { get; }
    public UserManager UserManager { get; }
    public RoleManager RoleManager { get; }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User.FindFirstValue(UserManager.Options.ClaimsIdentity.RoleClaimType) == PermissionPolicies.AdministratorsRequirement)
        {
            context.Succeed(requirement);
            return;
        }
        var currentUser = await UserManager.GetUserAsync(context.User);
        var httpcontext = (context.Resource as HttpContext)!;
        var currentArea = httpcontext.Request.RouteValues["area"] as string;
        var currentPage = (httpcontext.Request.RouteValues["page"] as string)!;
        if (await UserManager.HasMenuPermissionAsync(currentUser, currentArea, currentPage) || await UserManager.HasButtonPermissionAsync(currentUser, currentArea, currentPage))
        {
            context.Succeed(requirement);
            return;
        }
    }

}
