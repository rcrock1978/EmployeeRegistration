namespace Members.Domain.Members;

public record EmergencyContact
{
    public string ContactName { get; init; } = null!;
    public string Relationship { get; init; } = null!;
    public string ContactNumber { get; init; } = null!;

    private EmergencyContact() { }

    public EmergencyContact(string contactName, string relationship, string contactNumber)
    {
        ContactName = contactName;
        Relationship = relationship;
        ContactNumber = contactNumber;
    }
}
