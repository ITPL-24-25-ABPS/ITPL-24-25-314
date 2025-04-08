using ServerManager.Endpoints;
using ServerManager.Servers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ServerController>();

var app = builder.Build();

app.MapServerEndpoints();

app.Run();