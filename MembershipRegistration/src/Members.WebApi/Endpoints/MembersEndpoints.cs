using Members.Application.Common.Results;
using Members.Application.Features.Members.GetMemberById;
using Members.Application.Features.Members.ListMembers;
using Members.Application.Features.Members.RegisterMember;
using Members.Application.Features.Members.UpdateMember;
using Members.Application.Common.Messaging;
using Members.WebApi.Infrastructure;
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
            [FromServices] MemberOwnerAuthorizationFilter ownerFilter,
            CancellationToken cancellationToken) =>
        {
            var isAuthorized = await ownerFilter.IsOwnerOrAdminAsync(id, cancellationToken);
            if (!isAuthorized)
            {
                return Results.Forbid();
            }

            var query = new GetMemberByIdQuery(id);
            var result = await sender.Send(query, cancellationToken);

            if (result.IsSuccess)
            {
                return Results.Ok(new
                {
                    isSuccess = true,
                    value = result.Value,
                    error = (object?)null
                });
            }

            return Results.NotFound(new
            {
                isSuccess = false,
                value = (object?)null,
                error = result.Error
            });
        })
        .RequireAuthorization()
        .WithName("GetMemberById");

        app.MapGet("/api/members", async (
            [FromServices] ISender sender,
            CancellationToken cancellationToken,
            [FromQuery] string? lastName,
            [FromQuery] string? email,
            [FromQuery] string? employeeLevel,
            [FromQuery] DateTime? createdDateFrom,
            [FromQuery] DateTime? createdDateTo,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20) =>
        {
            var query = new ListMembersQuery(
                lastName, email, employeeLevel,
                createdDateFrom, createdDateTo,
                page, pageSize);

            var result = await sender.Send(query, cancellationToken);

            return Results.Ok(new
            {
                isSuccess = true,
                value = result.Value,
                error = (object?)null
            });
        })
        .RequireAuthorization("HRAdminOnly")
        .WithName("ListMembers");

        app.MapPut("/api/members/{id:guid}", async (
            Guid id,
            [FromBody] UpdateMemberCommand command,
            [FromServices] ISender sender,
            CancellationToken cancellationToken) =>
        {
            if (id != command.Id)
            {
                return Results.BadRequest(new
                {
                    isSuccess = false,
                    value = (object?)null,
                    error = new AppError(
                        "Validation.Mismatch",
                        "The ID in the URL does not match the ID in the request body.")
                });
            }

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

            if (result.Error?.Code == "NotFound")
            {
                return Results.NotFound(new
                {
                    isSuccess = false,
                    value = (object?)null,
                    error = result.Error
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
        .RequireAuthorization("HRAdminOnly")
        .WithName("UpdateMember");
    }
}
