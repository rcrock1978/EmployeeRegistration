using Members.Application.Common.Results;
using Members.Application.Features.Members.RegisterMember;
using Members.Application.Common.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace Members.WebApi.Endpoints;

public static class MembersEndpoints
{
    public static void MapMembersEndpoints(this WebApplication app)
    {
        app.MapPost("/api/members", async (
            [FromBody] RegisterMemberCommand command,
            [FromServices] ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);

            if (result.IsSuccess)
            {
                return Results.Created($"/api/members/{result.Value.Id}", new
                {
                    isSuccess = true,
                    value = result.Value,
                    error = (object?)null
                });
            }

            if (result.Error?.Code == "Conflict.DuplicateEmail")
            {
                return Results.Conflict(new
                {
                    isSuccess = false,
                    value = (object?)null,
                    error = result.Error
                });
            }

            return Results.BadRequest(new
            {
                isSuccess = false,
                value = (object?)null,
                error = result.Error
            });
        })
        .AllowAnonymous()
        .WithName("RegisterMember");

        app.MapGet("/api/members/{id:guid}", async (
            Guid id,
            [FromServices] ISender sender,
            CancellationToken cancellationToken) =>
        {
            return Results.Ok(new { isSuccess = true, value = (object?)null, error = (object?)null });
        })
        .RequireAuthorization("MemberOrHRAdmin")
        .WithName("GetMemberById");

        app.MapGet("/api/members", async (
            [FromServices] ISender sender,
            CancellationToken cancellationToken) =>
        {
            return Results.Ok(new { isSuccess = true, value = (object?)null, error = (object?)null });
        })
        .RequireAuthorization("HRAdminOnly")
        .WithName("ListMembers");

        app.MapPut("/api/members/{id:guid}", async (
            Guid id,
            [FromBody] object command,
            [FromServices] ISender sender,
            CancellationToken cancellationToken) =>
        {
            return Results.Ok(new { isSuccess = true, value = (object?)null, error = (object?)null });
        })
        .RequireAuthorization("HRAdminOnly")
        .WithName("UpdateMember");
    }
}
