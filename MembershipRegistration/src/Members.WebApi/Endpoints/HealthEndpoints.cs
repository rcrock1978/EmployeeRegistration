using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Members.Infrastructure.Persistence;

namespace Members.WebApi.Endpoints;

public static class HealthEndpoints
{
    public static void MapHealthEndpoints(this WebApplication app)
    {
        app.MapGet("/health/live", (HttpContext context) =>
        {
            var correlationId = context.Items["CorrelationId"] as string ?? Guid.NewGuid().ToString("N");

            return Results.Ok(new
            {
                status = "Healthy",
                timestamp = DateTime.UtcNow,
                correlationId
            });
        })
        .AllowAnonymous()
        .WithName("HealthLive");

        app.MapGet("/health/ready", async (MembersDbContext db, HttpContext context) =>
        {
            var correlationId = context.Items["CorrelationId"] as string ?? Guid.NewGuid().ToString("N");
            var dbConnected = false;
            var errors = new List<string>();

            try
            {
                dbConnected = await db.Database.CanConnectAsync();
            }
            catch (Exception ex)
            {
                errors.Add($"Database connection failed: {ex.Message}");
            }

            if (dbConnected)
            {
                return Results.Ok(new
                {
                    status = "Healthy",
                    timestamp = DateTime.UtcNow,
                    correlationId,
                    checks = new
                    {
                        database = new { status = "Healthy", latency = 0 }
                    }
                });
            }

            return Results.Json(
                new
                {
                    status = "Unhealthy",
                    timestamp = DateTime.UtcNow,
                    correlationId,
                    checks = new
                    {
                        database = new { status = "Unhealthy", errors }
                    }
                },
                statusCode: 503);
        })
        .AllowAnonymous()
        .WithName("HealthReady");
    }
}
