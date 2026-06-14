namespace Members.Domain.Members;

public record GovernmentIds
{
    public string Tin { get; init; } = null!;
    public string Sss { get; init; } = null!;

    private GovernmentIds() { }

    public GovernmentIds(string tin, string sss)
    {
        Tin = tin;
        Sss = sss;
    }
}
