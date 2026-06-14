using Microsoft.EntityFrameworkCore;
using Members.Infrastructure.Persistence;

namespace Members.WebApi.Endpoints;

public static class HealthEndpoints
{
    public static void MapHealthEndpoints(this WebApplication app)
    {
        app.MapGet("/health/live", () => Results.Ok("Healthy"))
            .AllowAnonymous()
            .WithName("HealthLive");

        app.MapGet("/health/ready", async (MembersDbContext db) =>
        {
            try
            {
                await db.Database.CanConnectAsync();
                return Results.Ok("Healthy");
            }
            catch
            {
                return Results.StatusCode(503);
            }
        })
        .AllowAnonymous()
        .WithName("HealthReady");
    }
}
