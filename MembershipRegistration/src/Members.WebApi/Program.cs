using System.Security.Cryptography;
using System.Text;
using FluentValidation;
using Members.Application.Common;
using Members.Application.Common.Behaviors;
using Members.Application.Common.Messaging;
using Members.Infrastructure.Persistence;
using Members.Infrastructure.Security;
using Members.WebApi.Endpoints;
using Members.WebApi.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<AuditInterceptor>();
builder.Services.AddScoped<Members.Domain.Members.IMemberRepository, Members.Infrastructure.Persistence.MemberRepository>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

var encryptionOptions = builder.Configuration
    .GetSection(EncryptionOptions.SectionName)
    .Get<EncryptionOptions>() ?? new EncryptionOptions();

var encryptionKey = string.IsNullOrEmpty(encryptionOptions.Base64EncodedKey)
    ? SHA256.HashData(Encoding.UTF8.GetBytes(encryptionOptions.DevPassphrase))
    : Convert.FromBase64String(encryptionOptions.Base64EncodedKey);

builder.Services.AddSingleton<IEncryptionService>(
    _ => new AesGcmEncryptionService(encryptionKey));

builder.Services.AddDbContext<MembersDbContext>((sp, options) =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"))
        .AddInterceptors(sp.GetRequiredService<AuditInterceptor>()));

var authOptions = builder.Configuration
    .GetSection(AuthOptions.SectionName)
    .Get<AuthOptions>() ?? new AuthOptions();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        if (authOptions.UseDevelopmentKey)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(authOptions.DevelopmentSigningKey));
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ClockSkew = TimeSpan.FromMinutes(authOptions.TokenValidationClockSkewMinutes)
            };
        }
        else
        {
            options.Authority = authOptions.Authority;
            options.Audience = authOptions.Audience;
            options.MetadataAddress = authOptions.MetadataUrl;
            options.RequireHttpsMetadata = authOptions.RequireHttpsMetadata;
            options.TokenValidationParameters.ClockSkew =
                TimeSpan.FromMinutes(authOptions.TokenValidationClockSkewMinutes);
        }
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("HRAdminOnly", policy =>
        policy.RequireRole("HRAdmin"));
    options.AddPolicy("MemberOrHRAdmin", policy =>
        policy.RequireAuthenticatedUser());
});

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

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthEndpoints();
app.MapMembersEndpoints();

app.Run();
