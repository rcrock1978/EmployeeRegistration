using System.Text.Json;
using Members.Application.Common;
using Microsoft.Extensions.Logging;

namespace Members.Infrastructure.Logging;

public sealed class MemberSubmissionLogger : IMemberSubmissionLogger
{
    private static readonly Action<ILogger, string, Exception?> LogSubmissionWritten =
        LoggerMessage.Define<string>(LogLevel.Information, new EventId(1, "SubmissionLogged"),
            "Member submission logged to {FilePath}");

    private static readonly Action<ILogger, string, string, string, Exception?> LogSubmissionFailed =
        LoggerMessage.Define<string, string, string>(LogLevel.Error, new EventId(2, "SubmissionLogFailed"),
            "Failed to write member submission log for {FirstName} {LastName} ({MemberId})");

    private readonly ILogger<MemberSubmissionLogger> _logger;
    private static readonly string MembersLogDir = Path.Combine("logs", "members");
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public MemberSubmissionLogger(ILogger<MemberSubmissionLogger> logger)
    {
        _logger = logger;
    }

    public async Task LogSubmissionAsync(Guid memberId, string firstName, string lastName, object submissionData)
    {
        try
        {
            Directory.CreateDirectory(MembersLogDir);

            var sanitizedFirst = SanitizeFileName(firstName);
            var sanitizedLast = SanitizeFileName(lastName);
            var fileName = $"{sanitizedFirst}_{sanitizedLast}_{memberId}.log";
            var filePath = Path.Combine(MembersLogDir, fileName);

            var json = JsonSerializer.Serialize(new
            {
                loggedAt = DateTime.UtcNow,
                memberId = memberId.ToString(),
                firstName,
                lastName,
                data = submissionData
            }, JsonOptions);

            await File.WriteAllTextAsync(filePath, json + Environment.NewLine);

            LogSubmissionWritten(_logger, filePath, null);
        }
        catch (Exception ex)
        {
            LogSubmissionFailed(_logger, firstName, lastName, memberId.ToString(), ex);
        }
    }

    private static string SanitizeFileName(string name)
    {
        var invalid = Path.GetInvalidFileNameChars();
        return string.Join("_", name.Where(c => !invalid.Contains(c))).ToLowerInvariant();
    }
}
