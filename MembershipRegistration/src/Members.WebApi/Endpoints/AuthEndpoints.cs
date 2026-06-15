using Members.Application.Common.Messaging;
using Members.Application.Common.Results;
using Members.Application.Features.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Members.WebApi.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/api/auth/login", async (
            [FromBody] LoginRequest request,
            [FromServices] ISender sender,
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return Results.BadRequest(new
                {
                    isSuccess = false,
                    value = (object?)null,
                    error = new { code = "Auth.InvalidCredentials", message = "Email and password are required." }
                });
            }

            var command = new LoginCommand(request.Email, request.Password);
            var result = await sender.Send(command, cancellationToken);

            if (result.IsSuccess)
            {
                return Results.Ok(new
                {
                    isSuccess = true,
                    value = result.Value,
                    error = (object?)null
                });
            }

            return Results.Json(new
            {
                isSuccess = false,
                value = (object?)null,
                error = new { code = "Auth.InvalidCredentials", message = "Invalid email or password." }
            }, statusCode: 401);
        })
        .AllowAnonymous()
        .WithName("Login");
    }
}
