using System.ComponentModel.DataAnnotations;

namespace PPE.ModelDto;
/// <summary>
/// 登录请求信息
/// </summary>
public class LoginDto
{
    /// <summary>
    /// 账号
    /// </summary>
    /// <value></value>
    [Display(Name = "账号", Prompt = "user name")]
    [StringLength(20, MinimumLength = 5, ErrorMessage = "{0} 必须为 {2} 到 {1} 个字符间")]
    [Required(ErrorMessage = "{0} 不能为空")]
    public string UserName { get; set; } = default!;

    /// <summary>
    /// 密码
    /// </summary>
    /// <value></value>
    [Display(Name = "密码")]
    [StringLength(20, MinimumLength = 5, ErrorMessage = "{0} 必须为 {2} 到 {1} 个字符间")]
    [Required(ErrorMessage = "{0} 不能为空")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = default!;

    /// <summary>
    /// 验证码
    /// </summary>
    /// <value></value>
    [Display(Name = "验证码")]
    [StringLength(4, MinimumLength = 4, ErrorMessage = "{0} 必须为 {1} 个字符间")]
    [Required(ErrorMessage = "{0} 不能为空")]
    public string VerifyCode { get; set; } = default!;

    public string? TwoFactorCode { get; init; }

    public string? TwoFactorRecoveryCode { get; init; }
}