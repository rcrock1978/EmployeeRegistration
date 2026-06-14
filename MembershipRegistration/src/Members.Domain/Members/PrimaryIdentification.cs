namespace Members.Domain.Members;

public record PrimaryIdentification
{
    public string Type { get; init; } = null!;
    public string Number { get; init; } = null!;
    public DateTime IssueDate { get; init; }
    public DateTime ExpiryDate { get; init; }
    public string IssueCountry { get; init; } = null!;

    private PrimaryIdentification() { }

    public PrimaryIdentification(string type, string number, DateTime issueDate, DateTime expiryDate, string issueCountry)
    {
        Type = type;
        Number = number;
        IssueDate = issueDate;
        ExpiryDate = expiryDate;
        IssueCountry = issueCountry;
    }
}
