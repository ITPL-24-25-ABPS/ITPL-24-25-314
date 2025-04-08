namespace Shared.ApiSchema.ServerManager;

public class AddServerModel
{
    public required string ServerName { get; set; }
    public required bool IsPrivate { get; set; }
    public string? Passcode { get; set; }
}