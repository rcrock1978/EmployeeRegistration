namespace Members.Domain.Members;

public record SpouseInfo
{
    public string FirstName { get; init; } = null!;
    public string? MiddleName { get; init; }
    public string LastName { get; init; } = null!;

    private SpouseInfo() { }

    public SpouseInfo(string firstName, string? middleName, string lastName)
    {
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
    }
}

public record FatherInfo
{
    public string FirstName { get; init; } = null!;
    public string? MiddleName { get; init; }
    public string LastName { get; init; } = null!;
    public string? Suffix { get; init; }

    private FatherInfo() { }

    public FatherInfo(string firstName, string? middleName, string lastName, string? suffix)
    {
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        Suffix = suffix;
    }
}

public record RelatedPersons
{
    public SpouseInfo? Spouse { get; init; }
    public string? MotherMaidenName { get; init; }
    public FatherInfo? Father { get; init; }

    private RelatedPersons() { }

    public RelatedPersons(SpouseInfo? spouse, string? motherMaidenName, FatherInfo? father)
    {
        Spouse = spouse;
        MotherMaidenName = motherMaidenName;
        Father = father;
    }
}
