using Microsoft.AspNetCore.Identity;
using PPE.Core;
using PPE.DataModel;
using PPE.Model.Shared;
using PPE.ModelDto;

namespace PPE.IDAL;
/// <summary>
/// 用户数据访问操作接口
/// </summary>
public interface IUserRepository : IUserLoginStore<Base_User>,
IUserClaimStore<Base_User>,
IUserPasswordStore<Base_User>,
IUserSecurityStampStore<Base_User>,
IUserEmailStore<Base_User>,
IUserLockoutStore<Base_User>,
IUserPhoneNumberStore<Base_User>,
IQueryableUserStore<Base_User>,
IUserTwoFactorStore<Base_User>,
IUserAuthenticationTokenStore<Base_User>,
IUserAuthenticatorKeyStore<Base_User>,
IUserTwoFactorRecoveryCodeStore<Base_User>,
IUserRoleStore<Base_User>
{
    IdentityFactory Identity { get; }
    /// <summary>
    /// 导入用户
    /// </summary>
    /// <param name="user">实体实体</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IdentityResult> ImportAsync(Base_User user, CancellationToken cancellationToken);

    /// <summary>
    /// 系统菜单是否授权给用户
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <param name="menu">系统菜单实体</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> IsMenuInUserAsync(Base_User user, Base_Menu menu, CancellationToken cancellationToken);

    /// <summary>
    /// 添加系统菜单授权
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <param name="menu">系统菜单实体</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task AddMenuToUserasync(Base_User user, Base_Menu menu, CancellationToken cancellationToken);

    /// <summary>
    /// 移除系统菜单授权
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <param name="menu">系统菜单实体</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RemoveMenuFromUserasync(Base_User user, Base_Menu menu, CancellationToken cancellationToken);

    /// <summary>
    /// 查询用户授权系统菜单
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IList<Base_Menu>?> FindMenusByUserAsync(Base_User user, CancellationToken cancellationToken);

    /// <summary>
    /// 操作按钮是否授权给用户
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <param name="button">操作按钮实体</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> IsButtonInUserAsync(Base_User user, Base_Button button, CancellationToken cancellationToken);

    /// <summary>
    /// 添加操作按钮授权
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <param name="button">操作按钮实体</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task AddButtonToUserAsync(Base_User user, Base_Button button, CancellationToken cancellationToken);

    /// <summary>
    /// 移除操作按钮授权
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <param name="button">操作按钮实体</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RemoveButtonFromUserAsync(Base_User user, Base_Button button, CancellationToken cancellationToken);

    /// <summary>
    /// 查询用户授权操作按钮
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IList<Base_Button>?> FindButtonsByUserAsync(Base_User user, CancellationToken cancellationToken);

    /// <summary>
    /// 查询指定系统菜单的用户授权操作按钮
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <param name="menu">系统菜单</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IList<Base_Button>?> FindButtonsByUserAsync(Base_User user, Base_Menu menu, CancellationToken cancellationToken);

    /// <summary>
    /// 更新用户登录信息
    /// </summary>
    /// <param name="user">登录用户</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IdentityResult> UpdateLoginInfoAsync(Base_User user, CancellationToken cancellationToken);

    /// <summary>
    /// 查询单个用户授权系统菜单
    /// </summary>
    /// <param name="user">用户实例</param>
    /// <param name="menu">系统菜单实例</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Base_UserMenu?> FindUserMenuAsync(Base_User user, Base_Menu menu, CancellationToken cancellationToken);

    /// <summary>
    /// 添加系统菜单授权
    /// </summary>
    /// <param name="user">用户主键</param>
    /// <param name="menu">系统菜单实例</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task AddMenuToUserAsync(Base_User user, Base_Menu menu, CancellationToken cancellationToken);

    /// <summary>
    /// 移除系统菜单授权
    /// </summary>
    /// <param name="user">用户实例</param>
    /// <param name="menu">系统菜单实例</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RemoveMenuFromUserAsync(Base_User user, Base_Menu menu, CancellationToken cancellationToken);

    /// <summary>
    /// 操作按钮是否授权给用户
    /// </summary>
    /// <param name="user">用户实例</param>
    /// <param name="buttonId">操作按钮主键</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> IsButtonInUserAsync(Base_User user, string buttonId, CancellationToken cancellationToken);

    /// <summary>
    /// 查询单个用户授权操作按钮
    /// </summary>
    /// <param name="user">用户实例</param>
    /// <param name="buttonId">操作按钮主键</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Base_UserButton?> FindUserButtonAsync(Base_User user, string buttonId, CancellationToken cancellationToken);

    /// <summary>
    /// 添加操作按钮授权
    /// </summary>
    /// <param name="user">用户实例</param>
    /// <param name="buttonId">操作按钮主键</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task AddButtonToUserAsync(Base_User user, string buttonId, CancellationToken cancellationToken);

    /// <summary>
    /// 移除操作按钮授权
    /// </summary>
    /// <param name="user">用户实例</param>
    /// <param name="buttonId">操作按钮主键</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RemoveButtonFromUserAsync(Base_User user, string buttonId, CancellationToken cancellationToken);

    /// <summary>
    /// 查询用户所有授权操作按钮
    /// </summary>
    /// <param name="user">用户实例</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IList<Base_Button>?> FindButtonsAsync(Base_User user, CancellationToken cancellationToken);

    /// <summary>
    /// 查询某个系统菜单的所有授权操作按钮
    /// </summary>
    /// <param name="user">用户实例</param>
    /// <param name="menu">系统菜单实例</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IList<OperationButtonDto>?> FindButtonsAsync(Base_User user, Base_Menu menu, CancellationToken cancellationToken);

    /// <summary>
    /// 查询某个系统菜单的所有授权操作按钮
    /// </summary>
    /// <param name="user">用户实例</param>
    /// <param name="menu">系统菜单实例</param>
    /// <param name="groups">操作按钮分组数组</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IList<OperationButtonDto>?> FindButtonsAsync(Base_User user, Base_Menu menu, ButtonGroup[]? groups = null, CancellationToken cancellationToken = default);


    /// <summary>
    /// 是否有系统菜单授权
    /// </summary>
    /// <param name="user">登录用户 <see cref="Base_User"/></param>
    /// <param name="normalizedArea">区域标准值</param>
    /// <param name="normalizedPage">页面标准值</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> HasMenuPermissionAsync(Base_User user, string? normalizedArea, string normalizedPage, CancellationToken cancellationToken);

    /// <summary>
    /// 用户是否有指定操作按钮权限
    /// </summary>
    /// <param name="user">登录用户 <see cref="Base_User"/></param>
    /// <param name="normalizedArea">区域标准值</param>
    /// <param name="normalizedUrl">路径标准值</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> HasButtonPermissionAsync(Base_User user, string? normalizedArea, string normalizedUrl, CancellationToken cancellationToken);

    /// <summary>
    /// 分页查询
    /// </summary>
    /// <param name="parameter"></param>
    /// <param name="isAdmin"></param>
    /// <param name="isDelete"></param>
    /// <param name="isLocked"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<DataTableResult<UserListDto>> FindPageAsync(DataTableParameter parameter, bool? isAdmin, bool? isDelete, bool? isLocked, CancellationToken cancellationToken);

    /// <summary>
    /// 授权管理分页查询
    /// </summary>
    /// <param name="parameter"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<DataTableResult<UserPermitListDto>> FindPermitPageAsync(DataTableParameter parameter, CancellationToken cancellationToken);

    /// <summary>
    /// 获取系统导航系统菜单
    /// </summary>
    /// <param name="user">登录用户</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IList<Base_Menu>?> FindNavigationsAsync(Base_User user, CancellationToken cancellationToken);

    /// <summary>
    /// 获取某个指定授权操作按钮
    /// </summary>
    /// <param name="user">登录用户 <see cref="Base_User"/></param>
    /// <param name="area">区域</param>
    /// <param name="url">操作地址</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Base_Button?> GetPermitButtonAsync(Base_User user, string? normalizedArea, string normalizedUrl, CancellationToken cancellationToken);

    /// <summary>
    /// 是否有指定的操作权限
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="normalizedArea">区域标准值</param>
    /// <param name="normalizedUrl">操作路径标准值</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> HasOperationAsync(Base_User user, string? normalizedArea, string normalizedUrl, CancellationToken cancellationToken);
}

