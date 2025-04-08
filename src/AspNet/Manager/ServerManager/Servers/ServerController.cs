using ServerManager.Endpoints;
using Shared.ApiSchema.Responses;
using Shared.ApiSchema.ServerManager;
using Shared.EntityFramework.Entities;

namespace ServerManager.Servers;

public class ServerController(ILogger<ServerController> logger)
{
    private List<ServerEntry> Servers { get; set; } = new();

    public List<ServerEntry> GetServers()
    {
        return Servers;
    }


    public ResponseModel<ServerEntry> CreateServer(AddServerModel model, string userName)
    {
        int port = 7100 + Servers.Count;
        ServerEntry e = new ServerEntry()
        {
            ServerName = model.ServerName,
            Owner = userName,
            Port = port,
            Players = new List<string>(),
            IsOnline = true,
            IsPrivate = model.IsPrivate,
            Passcode = model.Passcode
        };
        Servers.Add(e);
        e.StartProcess();

        try
        {
            Servers.Add(e);
            return new ResponseModel<ServerEntry>
            {
                Success = true,
                Entity = e,
                Message = "Server created successfully"
            };
        }
        catch (Exception ex)
        {
            return new ResponseModel<ServerEntry>
            {
                Success = false,
                Message = "Error creating server: " + ex.Message,
                Exception = ex
            };
        }
    }

    public bool JoinServer(string serverName, string playerName, string passCode)
    {
        return true;
    }

    public bool KillServer(KillServerModel model, string username)
    {
        var e = Servers.FirstOrDefault(s => s.ServerName == model.ServerName);
        if (e == null)
            return false;
        if (e.Owner != username)
            return false;
        return false;
        try
        {
            var entity = Servers.FirstOrDefault(s => s.ServerName == e.ServerName);
            if (entity == null)
                return false;
            entity.KillProcess();
            Servers.Remove(entity);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogCritical("Exception while killing server: \n" + ex.Message);
            return false;
        }
    }


}