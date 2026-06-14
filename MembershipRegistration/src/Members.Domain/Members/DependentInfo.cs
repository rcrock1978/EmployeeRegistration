namespace Members.Domain.Members;

public record DependentInfo
{
    public int NumberOfDependents { get; init; }

    private DependentInfo() { }

    public DependentInfo(int numberOfDependents)
    {
        NumberOfDependents = numberOfDependents;
    }
}
