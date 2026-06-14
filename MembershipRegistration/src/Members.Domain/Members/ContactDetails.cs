namespace Members.Domain.Members;

public record ContactDetails
{
    public string EmailAddress { get; init; } = null!;
    public string ContactNumber { get; init; } = null!;

    private ContactDetails() { }

    public ContactDetails(string emailAddress, string contactNumber)
    {
        EmailAddress = emailAddress;
        ContactNumber = contactNumber;
    }
}
