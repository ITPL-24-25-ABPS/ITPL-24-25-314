namespace Shared.ApiSchema.ServerManager;

public class CreateServerResponse
{
    public required string ServerName { get; set; }
    public required int Port { get; set; }
    public required string Owner { get; set; }
    public required List<string> Players { get; set; }
}