using System.ComponentModel.DataAnnotations;

namespace PPE.ModelDto;
/// <summary>
/// 注册请求信息
/// </summary>
public class RegisterDto
{
    /// <summary>
    /// 姓名
    /// </summary>
    /// <value></value>
    [Display(Name = "姓名", Prompt = "姓名")]
    public string? RealName { get; set; }
    /// <summary>
    /// 账号
    /// </summary>
    /// <value></value>
    [Display(Name = "账号", Prompt = "账号")]
    [Required(ErrorMessage = "{0} 不能为空")]
    public string? UserName { get; set; }
    /// <summary>
    /// 邮箱
    /// </summary>
    /// <value></value>
    [Display(Name = "邮箱", Prompt = "邮箱")]
    [EmailAddress(ErrorMessage = "{0} 格式错误")]
    public string? Email { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    /// <value></value>
    [Display(Name = "密码", Prompt = "密码")]
    [Required(ErrorMessage = "{0} 不能为空")]
    [DataType(DataType.Password)]
    [StringLength(20, MinimumLength = 5, ErrorMessage = "{0} 长度必须是 {2} 到 {1} 个字符。")]
    public string? Password { get; set; }

    /// <summary>
    /// 确认
    /// </summary>
    /// <value></value>
    [Display(Name = "确认密码", Prompt = "确认密码")]
    [Compare(nameof(Password))]
    [DataType(DataType.Password)]
    public string? ConfirmPassword { get; set; }
}