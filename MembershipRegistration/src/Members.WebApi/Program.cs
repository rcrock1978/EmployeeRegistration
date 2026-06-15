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
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {CorrelationId,-36} {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(
        path: "logs/optodev-members-.log",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {CorrelationId} {Message:lj}{NewLine}{Exception}",
        retainedFileCountLimit: 90)
    .Enrich.FromLogContext()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddOpenApi();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new DateTimeUtcConverter());
});

builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<AuditInterceptor>();
builder.Services.AddScoped<Members.Domain.Members.IMemberRepository, Members.Infrastructure.Persistence.MemberRepository>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<MemberOwnerAuthorizationFilter>();
builder.Services.AddScoped<IMemberSubmissionLogger, Members.Infrastructure.Logging.MemberSubmissionLogger>();
builder.Services.AddScoped<IAdminUserRepository, Members.Infrastructure.Persistence.AdminUserRepository>();
builder.Services.AddScoped<IPasswordHasher, Members.Infrastructure.Security.PasswordHasher>();

var authOptions = builder.Configuration
    .GetSection(AuthOptions.SectionName)
    .Get<AuthOptions>() ?? new AuthOptions();

builder.Services.AddSingleton<IJwtTokenService>(
    _ => new Members.Infrastructure.Security.JwtTokenService(authOptions.DevelopmentSigningKey));

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
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));
    options.AddPolicy("AdminOrHRAdmin", policy =>
        policy.RequireAssertion(ctx =>
            ctx.User.IsInRole("Admin") || ctx.User.IsInRole("HRAdmin")));
    options.AddPolicy("MemberOrAdmin", policy =>
        policy.RequireAuthenticatedUser());
});

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(Members.Application.Common.Results.Result).Assembly);

#pragma warning disable CA2263
builder.Services.AddScoped<IPipelineBehavior, ValidationBehavior>();
builder.Services.AddScoped<IPipelineBehavior, LoggingBehavior>();
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

app.UseMiddleware<CorrelationIdMiddleware>();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthEndpoints();
app.MapMembersEndpoints();
app.MapAuthEndpoints();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<MembersDbContext>();
    await db.Database.MigrateAsync();
    await DataSeeder.SeedAsync(db);
    await DataSeeder.SeedAdminUsersAsync(db,
        scope.ServiceProvider.GetRequiredService<IPasswordHasher>());
}

app.Run();
