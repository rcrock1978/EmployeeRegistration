using Members.Domain.Members;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Members.Infrastructure.Persistence.Configurations;

public sealed class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable("Members");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property<uint>("xmin")
            .HasColumnType("xid")
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(50);
        builder.Property(x => x.EmailAddress).HasMaxLength(320);

        builder.OwnsOne(x => x.PersonName, pn =>
        {
            pn.Property(p => p.Title).HasMaxLength(50);
            pn.Property(p => p.FirstName).HasMaxLength(100);
            pn.Property(p => p.MiddleName).HasMaxLength(100);
            pn.Property(p => p.LastName).HasMaxLength(100);
            pn.Property(p => p.Suffix).HasMaxLength(50);
            pn.Property(p => p.Alias).HasMaxLength(100);
        });

        builder.OwnsOne(x => x.Demographics, d =>
        {
            d.Property(p => p.PlaceOfBirth).HasMaxLength(200);
            d.Property(p => p.CountryOfBirth).HasMaxLength(100);
            d.Property(p => p.Nationality).HasMaxLength(100);
            d.Property(p => p.Gender).HasMaxLength(50);
            d.Property(p => p.CivilStatus).HasMaxLength(50);
            d.Property(p => p.Religion).HasMaxLength(100);
            d.Property(p => p.HighestEducationalAttainment).HasMaxLength(100);
        });

        builder.OwnsOne(x => x.ContactDetails, cd =>
        {
            cd.Property(p => p.EmailAddress).HasMaxLength(320);
            cd.Property(p => p.ContactNumber).HasMaxLength(50);
        });

        builder.OwnsOne(x => x.DependentInfo);

        builder.OwnsOne(x => x.RelatedPersons, rp =>
        {
            rp.OwnsOne(p => p.Spouse, s =>
            {
                s.Property(p => p.FirstName).HasMaxLength(100);
                s.Property(p => p.MiddleName).HasMaxLength(100);
                s.Property(p => p.LastName).HasMaxLength(100);
            });
            rp.Property(p => p.MotherMaidenName).HasMaxLength(200);
            rp.OwnsOne(p => p.Father, f =>
            {
                f.Property(p => p.FirstName).HasMaxLength(100);
                f.Property(p => p.MiddleName).HasMaxLength(100);
                f.Property(p => p.LastName).HasMaxLength(100);
                f.Property(p => p.Suffix).HasMaxLength(50);
            });
        });

        builder.OwnsOne(x => x.GovernmentIds, gi =>
        {
            gi.Property(p => p.Tin).HasMaxLength(200);
            gi.Property(p => p.Sss).HasMaxLength(200);
        });

        builder.OwnsOne(x => x.PrimaryIdentification, pi =>
        {
            pi.Property(p => p.Type).HasMaxLength(50);
            pi.Property(p => p.Number).HasMaxLength(100);
            pi.Property(p => p.IssueCountry).HasMaxLength(100);
        });

        builder.OwnsOne(x => x.CurrentAddress, ca =>
        {
            ca.Property(p => p.StreetNameAndNumber).HasMaxLength(200);
            ca.Property(p => p.City).HasMaxLength(100);
            ca.Property(p => p.PostalCode).HasMaxLength(20);
            ca.Property(p => p.Barangay).HasMaxLength(100);
            ca.Property(p => p.SubdivisionPurok).HasMaxLength(100);
            ca.Property(p => p.Province).HasMaxLength(100);
            ca.Property(p => p.Country).HasMaxLength(100);
            ca.Property(p => p.OwnerOrLessee).HasMaxLength(50);
        });

        builder.OwnsOne(x => x.PermanentAddress, pa =>
        {
            pa.OwnsOne(p => p.Address, a =>
            {
                a.Property(p => p.StreetNameAndNumber).HasMaxLength(200);
                a.Property(p => p.City).HasMaxLength(100);
                a.Property(p => p.PostalCode).HasMaxLength(20);
                a.Property(p => p.Barangay).HasMaxLength(100);
                a.Property(p => p.SubdivisionPurok).HasMaxLength(100);
                a.Property(p => p.Province).HasMaxLength(100);
                a.Property(p => p.Country).HasMaxLength(100);
                a.Property(p => p.OwnerOrLessee).HasMaxLength(50);
            });
        });

        builder.OwnsOne(x => x.EmergencyContact, ec =>
        {
            ec.Property(p => p.ContactName).HasMaxLength(200);
            ec.Property(p => p.Relationship).HasMaxLength(100);
            ec.Property(p => p.ContactNumber).HasMaxLength(50);
        });

        builder.OwnsOne(x => x.EmploymentDetails, ed =>
        {
            ed.Property(p => p.EmployeeLevel).HasMaxLength(50);
            ed.Property(p => p.CompanyTradeName).HasMaxLength(200);
            ed.Property(p => p.CompanyIdNumber).HasMaxLength(100);
            ed.Property(p => p.GrossIncome).HasColumnType("decimal(18,2)");
            ed.Property(p => p.IncomePeriod).HasMaxLength(50);
            ed.Property(p => p.Occupation).HasMaxLength(200);
        });

        builder.OwnsOne(x => x.Consent, c =>
        {
            c.Property(p => p.SignatureName).HasMaxLength(200);
        });

        builder.HasIndex(x => x.EmailAddress).IsUnique();
    }
}
