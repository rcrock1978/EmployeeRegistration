using System.Diagnostics;
using System.Text.RegularExpressions;
using Members.Application.Common.Results;
using Members.Application.Common.Messaging;
using Microsoft.Extensions.Logging;

namespace Members.Application.Common.Behaviors;

public sealed partial class LoggingBehavior : IPipelineBehavior
{
    private readonly ILogger<LoggingBehavior> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior> logger)
    {
        _logger = logger;
    }

    public async Task<Result<TResponse>> HandleAsync<TResponse>(
        object request,
        Func<Task<Result<TResponse>>> nextHandler,
        CancellationToken cancellationToken)
    {
        var requestName = request.GetType().Name;
        var requestType = request.GetType().FullName;
        var sanitizedRequest = RedactPii(request);

        _logger.LogTrace(">>> Entering handler for {RequestName} ({RequestType})", requestName, requestType);
        _logger.LogTrace("Request payload: {Request}", sanitizedRequest);

        var stopwatch = Stopwatch.StartNew();
        var result = await nextHandler();
        stopwatch.Stop();

        if (result.IsSuccess)
        {
            _logger.LogTrace("<<< Exiting handler for {RequestName} — success in {ElapsedMs}ms",
                requestName, stopwatch.ElapsedMilliseconds);
            _logger.LogInformation("Handled {RequestName} successfully in {ElapsedMs}ms",
                requestName, stopwatch.ElapsedMilliseconds);
        }
        else
        {
            _logger.LogTrace("<<< Exiting handler for {RequestName} — failed in {ElapsedMs}ms: {ErrorCode} - {ErrorMessage}",
                requestName, stopwatch.ElapsedMilliseconds, result.Error?.Code, result.Error?.Message);
            _logger.LogWarning("Handled {RequestName} with failure in {ElapsedMs}ms: {ErrorCode} - {ErrorMessage}",
                requestName, stopwatch.ElapsedMilliseconds, result.Error?.Code, result.Error?.Message);
        }

        return result;
    }

    private static string RedactPii(object request)
    {
        var serialized = System.Text.Json.JsonSerializer.Serialize(request);

        serialized = TinPattern().Replace(serialized, m =>
            $"{m.Value[..4]}***-***{m.Value[^4..]}");

        serialized = SssPattern().Replace(serialized, m =>
            $"{m.Value[..3]}*******{m.Value[^2..]}");

        return serialized;
    }

    [GeneratedRegex(@"\b\d{3}-\d{3}-\d{3}-\d{3}\b")]
    private static partial Regex TinPattern();

    [GeneratedRegex(@"\b\d{2}-\d{7}-\d{1}\b")]
    private static partial Regex SssPattern();
}
