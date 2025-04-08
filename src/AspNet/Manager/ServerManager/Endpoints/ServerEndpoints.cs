using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using ServerManager.Servers;
using Shared.ApiSchema.ServerManager;

namespace ServerManager.Endpoints;

public static class ServerEndpoints
{
    public static void MapServerEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/servers", async (
            [FromServices] ServerController controller) =>
        {
            return Results.Ok(controller.GetServers());
        });

        endpoints.MapGet("/joinServer", async (
            [FromBody] JoinServerModal modal,
            [FromServices] ServerController controller) =>
        {
            var entity = controller.GetServers().FirstOrDefault(s => s.ServerName == modal.ServerName);
            if(entity is null)
                return Results.NotFound("Server not found");
            if (entity.IsPrivate && entity.Passcode != modal.PassCode)
                return Results.Forbid();
            if (!entity.IsOnline)
                return Results.NotFound("Server not found");
            // TODO
            return Results.Ok();

        });

        endpoints.MapPost("/startServer", async (
            [FromServices] ServerController controller,
            [FromBody] AddServerModel model) =>
        {
            var result = controller.CreateServer(model, "");
            if(!result.Success || result.Entity is null)
                return Results.BadRequest(result.Message);

            return Results.Ok(new CreateServerResponse
            {
                ServerName = result.Entity.ServerName,
                Port = result.Entity.Port,
                Owner = result.Entity.Owner,
                Players = result.Entity.Players
            });

        });

        endpoints.MapDelete("/killServer", async (
            [FromServices] ServerController controller,
            [FromBody] KillServerModel model) =>
        {

            var result = controller.KillServer(model, "") ? Results.Ok("Server killed") : Results.NotFound("Server not found");

        });
    }
}