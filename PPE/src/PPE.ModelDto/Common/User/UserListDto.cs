using System.ComponentModel.DataAnnotations;

namespace PPE.ModelDto;

public class UserListDto
{
    public string? Id { get; set; }
    [Display(Name = "账号")]
    public string? UserName { get; set; }
    public string? NormalizedUserName { get; set; }
    [Display(Name = "姓名")]
    public string? RealName { get; set; }
    [Display(Name = "邮箱")]
    public string? Email { get; set; }
    public string? NormalizedEmail { get; set; }
    [Display(Name = "账号确认")]
    public bool EmailConfirmed { get; set; }
    [Display(Name = "电话")]
    public string? PhoneNumber { get; set; }
    [Display(Name = "电话确认")]
    public bool PhoneNumberConfirmed { get; set; }

    [Display(Name = "管理员")]
    public bool IsAdministrator { get; set; }

    [Display(Name = "登录次数")]
    public int AccessCount { get; set; }
    [Display(Name = "最后登录IP")]
    public string? AccessIP { get; set; }
    [Display(Name = "首次登录时间")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
    public DateTime? FirstAccess { get; set; }
    [Display(Name = "上次登录时间")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
    public DateTime? PreviousAccess { get; set; }
    [Display(Name = "最后登录时间")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
    public DateTime? LastAccess { get; set; }
    [Display(Name = "删除")]
    public bool IsDeleted { get; set; }
    [Display(Name = "说明")]
    public string? Description { get; set; }
    [Display(Name = "安全戳")]
    public string? SecurityStamp { get; set; }
    [Display(Name = "两种验证")]
    public bool TwoFactorEnabled { get; set; }
    [Display(Name = "锁定终止时间")]
    public DateTimeOffset? LockoutEnd { get; set; }
    [Display(Name = "允许锁定")]
    public bool LockoutEnabled { get; set; }
    [Display(Name = "登录失败次数")]
    public int AccessFailedCount { get; set; }

    public override string ToString()
    {
        return UserName ?? string.Empty;
    }
}
