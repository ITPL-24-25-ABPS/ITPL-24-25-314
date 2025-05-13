using ServerManager.Endpoints;
using ServerManager.Servers;

var builder = WebApplication.CreateBuilder(args);

var serverPath = builder.Configuration["ServerPaths:ServerPathWin"];
if (string.IsNullOrEmpty(serverPath))
{
    throw new EntryPointNotFoundException("Server path not found in configuration.");
}
ServerEntry.ServerPath = serverPath;

builder.Services.AddSingleton<ServerController>();

var app = builder.Build();

app.MapServerEndpoints();

app.Run();