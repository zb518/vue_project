using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication.BearerToken;

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
    [Required]
    public string UserName { get; set; } = default!;

    /// <summary>
    /// 密码
    /// </summary>
    /// <value></value>
    [Required]
    public string Password { get; set; } = default!;

    /// <summary>
    /// 验证码
    /// </summary>
    /// <value></value>
    [Required]
    public string VerifyCode { get; set; } = default!;

    public string? TwoFactorCode { get; init; }

    public string? TwoFactorRecoveryCode { get; init; }
}

/// <summary>
/// 验证账号请求信息
/// </summary>
public class ValidateUserRequest
{
    /// <summary>
    /// 账号
    /// </summary>
    public string? UserName { get; set; }
}

/// <summary>
/// 验证密码请求信息
/// </summary>
public class ValidatePasswordRequest : ValidateUserRequest
{
    /// <summary>
    /// 密码
    /// </summary>
    public string? Password { get; set; }
}

/// <summary>
/// 验证验证码请求信息
/// </summary>
public class ValidateVerifyCodeRequtest
{
    /// <summary>
    /// 验证码
    /// </summary>
    public string? VerifyCode { get; set; }
}



public class LoginResult
{
    public bool Succeeded { get; set; }
    public string? Message { get; set; }
    public AccessTokenResponse? AccessTokenResponse { get; set; }
}