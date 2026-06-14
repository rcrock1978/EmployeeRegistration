namespace Members.Domain.Members;

public record Demographics
{
    public DateTime DateOfBirth { get; init; }
    public string PlaceOfBirth { get; init; } = null!;
    public string CountryOfBirth { get; init; } = null!;
    public string Nationality { get; init; } = null!;
    public string Gender { get; init; } = null!;
    public string CivilStatus { get; init; } = null!;
    public string? Religion { get; init; }
    public string HighestEducationalAttainment { get; init; } = null!;

    private Demographics() { }

    public Demographics(DateTime dateOfBirth, string placeOfBirth, string countryOfBirth, string nationality,
        string gender, string civilStatus, string? religion, string highestEducationalAttainment)
    {
        DateOfBirth = dateOfBirth;
        PlaceOfBirth = placeOfBirth;
        CountryOfBirth = countryOfBirth;
        Nationality = nationality;
        Gender = gender;
        CivilStatus = civilStatus;
        Religion = religion;
        HighestEducationalAttainment = highestEducationalAttainment;
    }
}
