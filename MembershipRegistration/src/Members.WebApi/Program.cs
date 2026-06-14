using FluentValidation;
using Members.Application.Common.Behaviors;
using Members.Application.Common.Messaging;
using Members.Infrastructure.Persistence;
using Members.WebApi.Endpoints;
using Members.WebApi.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<AuditInterceptor>();
builder.Services.AddScoped<Members.Domain.Members.IMemberRepository, Members.Infrastructure.Persistence.MemberRepository>();

builder.Services.AddDbContext<MembersDbContext>((sp, options) =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"))
        .AddInterceptors(sp.GetRequiredService<AuditInterceptor>()));

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(Members.Application.Common.Results.Result).Assembly);

#pragma warning disable CA2263
builder.Services.AddSingleton(typeof(IPipelineBehavior), typeof(ValidationBehavior<>));
builder.Services.AddSingleton(typeof(IPipelineBehavior), typeof(LoggingBehavior<>));
#pragma warning restore CA2263
builder.Services.AddScoped<ISender, Sender>();

builder.Services.Scan(scan => scan
    .FromAssemblies(typeof(Members.Application.Common.Results.Result).Assembly)
    .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)))
    .AsImplementedInterfaces()
    .WithScopedLifetime()
    .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
    .AsImplementedInterfaces()
    .WithScopedLifetime());

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapHealthEndpoints();
app.MapMembersEndpoints();

app.Run();
