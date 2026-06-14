namespace Members.Domain.Members;

public record EmploymentDetails
{
    public string EmployeeLevel { get; init; } = null!;
    public string CompanyTradeName { get; init; } = null!;
    public string CompanyIdNumber { get; init; } = null!;
    public decimal GrossIncome { get; init; }
    public string IncomePeriod { get; init; } = null!;
    public string Occupation { get; init; } = null!;
    public DateTime HiredFrom { get; init; }
    public DateTime? HiredTo { get; init; }

    private EmploymentDetails() { }

    public EmploymentDetails(string employeeLevel, string companyTradeName, string companyIdNumber,
        decimal grossIncome, string incomePeriod, string occupation, DateTime hiredFrom, DateTime? hiredTo)
    {
        EmployeeLevel = employeeLevel;
        CompanyTradeName = companyTradeName;
        CompanyIdNumber = companyIdNumber;
        GrossIncome = grossIncome;
        IncomePeriod = incomePeriod;
        Occupation = occupation;
        HiredFrom = hiredFrom;
        HiredTo = hiredTo;
    }
}
