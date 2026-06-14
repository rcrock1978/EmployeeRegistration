using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Members.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EmailAddress = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    PersonName_Title = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PersonName_FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PersonName_MiddleName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PersonName_LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PersonName_Suffix = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PersonName_Alias = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Demographics_DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Demographics_PlaceOfBirth = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Demographics_CountryOfBirth = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Demographics_Nationality = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Demographics_Gender = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Demographics_CivilStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Demographics_Religion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Demographics_HighestEducationalAttainment = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ContactDetails_EmailAddress = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    ContactDetails_ContactNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DependentInfo_NumberOfDependents = table.Column<int>(type: "integer", nullable: false),
                    RelatedPersons_Spouse_FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RelatedPersons_Spouse_MiddleName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RelatedPersons_Spouse_LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RelatedPersons_MotherMaidenName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    RelatedPersons_Father_FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RelatedPersons_Father_MiddleName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RelatedPersons_Father_LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RelatedPersons_Father_Suffix = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    GovernmentIds_Tin = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    GovernmentIds_Sss = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PrimaryIdentification_Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PrimaryIdentification_Number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PrimaryIdentification_IssueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PrimaryIdentification_ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PrimaryIdentification_IssueCountry = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CurrentAddress_StreetNameAndNumber = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CurrentAddress_City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CurrentAddress_PostalCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CurrentAddress_Barangay = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CurrentAddress_SubdivisionPurok = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CurrentAddress_Province = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CurrentAddress_Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CurrentAddress_OwnerOrLessee = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CurrentAddress_OccupiedSince = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PermanentAddress_SameAsCurrent = table.Column<bool>(type: "boolean", nullable: false),
                    PermanentAddress_Address_StreetNameAndNumber = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PermanentAddress_Address_City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PermanentAddress_Address_PostalCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    PermanentAddress_Address_Barangay = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PermanentAddress_Address_SubdivisionPurok = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PermanentAddress_Address_Province = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PermanentAddress_Address_Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PermanentAddress_Address_OwnerOrLessee = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PermanentAddress_Address_OccupiedSince = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EmergencyContact_ContactName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    EmergencyContact_Relationship = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EmergencyContact_ContactNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EmploymentDetails_EmployeeLevel = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EmploymentDetails_CompanyTradeName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    EmploymentDetails_CompanyIdNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EmploymentDetails_GrossIncome = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    EmploymentDetails_IncomePeriod = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EmploymentDetails_Occupation = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    EmploymentDetails_HiredFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EmploymentDetails_HiredTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Consent_ConsentGiven = table.Column<bool>(type: "boolean", nullable: false),
                    Consent_Attestation = table.Column<bool>(type: "boolean", nullable: false),
                    Consent_SignatureName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Consent_SignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Members_EmailAddress",
                table: "Members",
                column: "EmailAddress",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Members");
        }
    }
}
