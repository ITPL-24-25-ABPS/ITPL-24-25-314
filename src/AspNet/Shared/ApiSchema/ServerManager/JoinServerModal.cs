namespace Shared.ApiSchema.ServerManager;

public class JoinServerModal
{
    public required string ServerName { get; set; }
    public string PassCode { get; set; } = "";
}