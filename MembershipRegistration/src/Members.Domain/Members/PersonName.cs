namespace Members.Domain.Members;

public record PersonName
{
    public string Title { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string? MiddleName { get; init; }
    public string LastName { get; init; } = null!;
    public string? Suffix { get; init; }
    public string? Alias { get; init; }

    private PersonName() { }

    public PersonName(string title, string firstName, string? middleName, string lastName, string? suffix, string? alias)
    {
        Title = title;
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        Suffix = suffix;
        Alias = alias;
    }
}
