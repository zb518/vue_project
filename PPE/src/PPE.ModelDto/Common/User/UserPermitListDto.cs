namespace PPE.ModelDto;

public class UserPermitListDto
{
    public string? Id { get; set; }
    public string? UserName { get; set; }
    public string? RealName { get; set; }
    public bool IsAdministrator { get; set; }
    public bool IsDeleted { get; set; }
    public string? Description { get; set; }
}