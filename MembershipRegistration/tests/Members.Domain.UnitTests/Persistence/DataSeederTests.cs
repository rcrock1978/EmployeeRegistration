using Members.Domain.Members;
using Members.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Members.Domain.UnitTests.Persistence;

public sealed class DataSeederTests : IDisposable
{
    private readonly MembersDbContext _context;

    public DataSeederTests()
    {
        var options = new DbContextOptionsBuilder<MembersDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new MembersDbContext(options);
        _context.Database.EnsureCreated();
    }

    [Fact]
    public async Task SeedAsync_WhenEmpty_Inserts1000Members()
    {
        await DataSeeder.SeedAsync(_context);

        Assert.Equal(1000, await _context.Set<Member>().CountAsync());
    }

    [Fact]
    public async Task SeedAsync_WhenNotEmpty_Skips()
    {
        await DataSeeder.SeedAsync(_context);
        var countAfterFirst = await _context.Set<Member>().CountAsync();

        await DataSeeder.SeedAsync(_context);
        var countAfterSecond = await _context.Set<Member>().CountAsync();

        Assert.Equal(1000, countAfterFirst);
        Assert.Equal(1000, countAfterSecond);
    }

    [Fact]
    public async Task SeedAsync_AllMembersHaveConsentTrue()
    {
        await DataSeeder.SeedAsync(_context);

        var members = await _context.Set<Member>().ToListAsync();
        Assert.All(members, m =>
        {
            Assert.True(m.Consent.ConsentGiven);
            Assert.True(m.Consent.Attestation);
            Assert.False(string.IsNullOrWhiteSpace(m.Consent.SignatureName));
        });
    }

    [Fact]
    public async Task SeedAsync_AllMembersHaveNonNullRequiredFields()
    {
        await DataSeeder.SeedAsync(_context);

        var members = await _context.Set<Member>().ToListAsync();
        Assert.All(members, m =>
        {
            Assert.False(string.IsNullOrWhiteSpace(m.EmailAddress));
            Assert.NotNull(m.PersonName);
            Assert.False(string.IsNullOrWhiteSpace(m.PersonName.FirstName));
            Assert.False(string.IsNullOrWhiteSpace(m.PersonName.LastName));
            Assert.NotNull(m.ContactDetails);
            Assert.False(string.IsNullOrWhiteSpace(m.ContactDetails.EmailAddress));
            Assert.False(string.IsNullOrWhiteSpace(m.ContactDetails.ContactNumber));
            Assert.NotNull(m.GovernmentIds);
            Assert.False(string.IsNullOrWhiteSpace(m.GovernmentIds.Tin));
            Assert.False(string.IsNullOrWhiteSpace(m.GovernmentIds.Sss));
            Assert.NotNull(m.CurrentAddress);
            Assert.NotNull(m.EmploymentDetails);
            Assert.NotNull(m.EmergencyContact);
            Assert.NotNull(m.DependentInfo);
        });
    }

    [Fact]
    public async Task SeedAsync_AllMembersHaveUniqueEmails()
    {
        await DataSeeder.SeedAsync(_context);

        var emails = await _context.Set<Member>()
            .Select(m => m.EmailAddress)
            .ToListAsync();

        Assert.Equal(emails.Count, emails.Distinct(StringComparer.OrdinalIgnoreCase).Count());
    }

    [Fact]
    public async Task SeedAsync_StatusDistribution_ContainsAllStatuses()
    {
        await DataSeeder.SeedAsync(_context);

        var statuses = await _context.Set<Member>()
            .Select(m => m.Status)
            .Distinct()
            .ToListAsync();

        Assert.Contains(MemberStatus.Submitted, statuses);
        Assert.Contains(MemberStatus.UnderReview, statuses);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
