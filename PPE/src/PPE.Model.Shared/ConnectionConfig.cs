namespace PPE.Model.Shared;

public class ConnectionConfig
{
    public string ConnectionName { get; set; } = "DefaultConnection";
    public ProviderType ProviderType { get; set; } = ProviderType.MySql;
    public string ConnectionString { get; set; } = "Server=localhost;Database=PPE-DB;UID=root;PWD=ASdf!@34;charset=utf8mb4";
}