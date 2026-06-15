namespace Members.Application.Common;

public interface IMemberSubmissionLogger
{
    Task LogSubmissionAsync(Guid memberId, string firstName, string lastName, object submissionData);
}
