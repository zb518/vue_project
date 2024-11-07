using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PPE.DataModel;

namespace PPE.BLL;
/// <summary>
/// 登录验证业务逻辑管理
/// </summary>
public class SignInManager : SignInManager<Base_User>
{
    public SignInManager(UserManager userManager, IHttpContextAccessor contextAccessor, UserClaimsPrincipalFactory claimsFactory, IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<Base_User>> logger, IAuthenticationSchemeProvider schemes, IUserConfirmation<Base_User> confirmation) : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
    {
        UserManager = userManager;
        ClaimsFactory = claimsFactory;
    }

    public new UserManager UserManager { get; set; }
    public new UserClaimsPrincipalFactory ClaimsFactory { get; set; }
}