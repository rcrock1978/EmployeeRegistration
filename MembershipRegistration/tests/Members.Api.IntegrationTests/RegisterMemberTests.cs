using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using Members.Api.IntegrationTests.Testing;

namespace Members.Api.IntegrationTests;

[Collection("Api")]
public sealed class RegisterMemberTests
{
    private readonly HttpClient _client;

    public RegisterMemberTests(ApiWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Post_valid_request_returns_201()
    {
        var request = CreateValidRequest("juan." + Guid.NewGuid() + "@example.com");

        var response = await _client.PostAsJsonAsync("/api/members", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = doc.RootElement;
        Assert.True(root.GetProperty("isSuccess").GetBoolean());
            Assert.NotEqual(Guid.Empty, root.GetProperty("value").GetProperty("id").GetGuid());
    }

    [Fact]
    public async Task Post_duplicate_email_returns_409()
    {
        var email = "duplicate." + Guid.NewGuid() + "@example.com";
        var request = CreateValidRequest(email);

        await _client.PostAsJsonAsync("/api/members", request);
        var response = await _client.PostAsJsonAsync("/api/members", request);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = doc.RootElement;
        Assert.False(root.GetProperty("isSuccess").GetBoolean());
        Assert.Equal("Conflict.DuplicateEmail", root.GetProperty("error").GetProperty("code").GetString());
    }

    [Fact]
    public async Task Post_missing_required_fields_returns_400()
    {
        var request = new { };

        var response = await _client.PostAsJsonAsync("/api/members", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = doc.RootElement;
        Assert.False(root.GetProperty("isSuccess").GetBoolean());
        Assert.Equal("Validation.Failed", root.GetProperty("error").GetProperty("code").GetString());
    }

    private static object CreateValidRequest(string email) => new
    {
        personalInfo = new
        {
            title = "Mr.",
            firstName = "Juan",
            middleName = "Protacio",
            lastName = "Dela Cruz",
            suffix = "",
            alias = "",
            dateOfBirth = "1990-05-12",
            placeOfBirth = "Manila",
            countryOfBirth = "PH",
            nationality = "Filipino",
            gender = "Male",
            civilStatus = "Married",
            religion = "",
            highestEducationalAttainment = "Bachelor's",
            numberOfDependents = 2
        },
        contactInfo = new
        {
            emailAddress = email,
            contactNumber = "+639170000000"
        },
        relatedPersons = new
        {
            spouse = new
            {
                firstName = "Maria",
                middleName = "Reyes",
                lastName = "Dela Cruz"
            },
            motherMaidenName = "Ana Bautista Reyes",
            father = new
            {
                firstName = "Pedro",
                middleName = "Santos",
                lastName = "Dela Cruz",
                suffix = "Sr."
            }
        },
        governmentIds = new
        {
            tin = "123-456-789-000",
            sss = "01-2345678-9"
        },
        primaryId = new
        {
            type = "Passport",
            number = "P1234567A",
            issueDate = "2021-01-10",
            expiryDate = "2031-01-09",
            issueCountry = "PH"
        },
        currentAddress = new
        {
            streetNameAndNumber = "123 Mabini St.",
            city = "Navotas",
            postalCode = "1485",
            barangay = "San Roque",
            subdivisionPurok = "Purok 2",
            province = "Metro Manila",
            country = "PH",
            ownerOrLessee = "Owner",
            occupiedSince = "2015-06-01"
        },
        permanentAddress = new
        {
            sameAsCurrent = true
        },
        emergencyContact = new
        {
            contactName = "Maria Dela Cruz",
            relationship = "Spouse",
            contactNumber = "+639170000001"
        },
        employment = new
        {
            employeeLevel = "RNF",
            companyTradeName = "OPTODEV, INC.",
            companyIdNumber = "OPT-00421",
            grossIncome = 45000,
            incomePeriod = "Monthly",
            occupation = "Technician",
            hiredFrom = "2019-12-01",
            hiredTo = (string?)null
        },
        consent = new
        {
            consentGiven = true,
            attestation = true,
            signatureName = "Juan P. Dela Cruz"
        }
    };
}
