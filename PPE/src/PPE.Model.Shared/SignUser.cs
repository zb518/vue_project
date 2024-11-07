namespace PPE.Model.Shared;
/// <summary>
/// 登录用户信息
/// </summary>
public class SignUser
{
    /// <summary>
    /// 无参构造
    /// </summary>
    public SignUser()
    {
    }

    /// <summary>
    /// 带参构造
    /// </summary>
    /// <param name="id">用户主键</param>
    /// <param name="userName">用户账号</param>
    /// <param name="realName">用户姓名</param>
    /// <param name="email">用户邮箱</param>
    /// <param name="securityStamp">用户安全戳</param>
    /// <param name="roles">用户所在角色</param>
    public SignUser(string? id, string? userName, string? realName, string? email, string? securityStamp, string? roles)
    {
        Id = id;
        UserName = userName;
        RealName = realName;
        Email = email;
        SecurityStamp = securityStamp;
        Roles = roles;
    }
    /// <summary>
    /// 用户主键
    /// </summary>
    /// <value></value>
    public string? Id { get; set; }
    /// <summary>
    /// 用户账号
    /// </summary>
    /// <value></value>
    public string? UserName { get; set; }
    /// <summary>
    /// 用户姓名
    /// </summary>
    /// <value></value>
    public string? RealName { get; set; }
    /// <summary>
    /// 用户邮箱
    /// </summary>
    /// <value></value>
    public string? Email { get; set; }
    /// <summary>
    /// 用户安全戳
    /// </summary>
    /// <value></value>
    public string? SecurityStamp { get; set; }
    /// <summary>
    /// 用户所在角色
    /// </summary>
    /// <value></value>
    public string? Roles { get; set; }
}
