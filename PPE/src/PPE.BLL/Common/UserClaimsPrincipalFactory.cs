using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using PPE.DataModel;
using System.Security.Claims;

namespace PPE.BLL;
/// <summary>
/// 
/// </summary>
public class UserClaimsPrincipalFactory : UserClaimsPrincipalFactory<Base_User, Base_Role>
{
    public UserClaimsPrincipalFactory(UserManager userManager, RoleManager roleManager, IOptions<IdentityOptions> options) : base(userManager, roleManager, options)
    {
        UserManager = userManager;
        RoleManager = roleManager;
    }
    public new UserManager UserManager { get; set; }
    public new RoleManager RoleManager { get; set; }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(Base_User user)
    {
        var id = await base.GenerateClaimsAsync(user).ConfigureAwait(false);
        if (user.RealName != null)
        {
            id.AddClaim(new Claim("Identity.RealName", user.RealName));
        }
        return id;
    }
}