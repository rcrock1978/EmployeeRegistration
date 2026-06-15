using Members.Domain.AdminUsers;
using Members.Domain.Members;
using Microsoft.EntityFrameworkCore;

namespace Members.Infrastructure.Persistence;

public static class DataSeeder
{
    private static readonly Random Rng = new();

    private static readonly string[] Titles = ["Mr.", "Ms.", "Mrs.", "Dr.", "Engr.", "Atty."];
    private static readonly string[] FirstNames =
    [
        "Juan", "Maria", "Jose", "Ana", "Pedro", "Rosa", "Carlos", "Luz", "Antonio", "Elena",
        "Miguel", "Luzviminda", "Ramon", "Gloria", "Eduardo", "Fe", "Manuel", "Teresa", "Francisco", "Corazon",
        "Dante", "Lourdes", "Rolando", "Edna", "Gilbert", "Mila", "Reynaldo", "Nenita", "Edwin", "Leticia",
        "Allan", "Merlinda", "Danilo", "Aurora", "Ricky", "Belinda", "Rogelio", "Cynthia", "Raffy", "Dolores",
        "Jeffrey", "Vilma", "Ferdinand", "Jocelyn", "Emmanuel", "Rosario", "Noel", "Maricel", "Sonny", "Cecilia",
        "Benny", "Leonora", "Cesar", "Fely", "Rudy", "Conchita", "Arnel", "Nelia", "Pablo", "Ester",
        "Mario", "Zenaida", "Efren", "Milagros", "Alberto", "Carolina", "Rene", "Ofelia", "Froilan", "Salvacion",
        "Lito", "Iluminada", "Gregorio", "Melinda", "Rodolfo", "Salome", "Nelson", "Adelaida", "Joel", "Visitacion",
        "Oscar", "Rosalinda", "Adrian", "Marilyn", "Rommel", "Anita", "Winston", "Nestor", "Edgardo", "Betty",
        "Raymond", "Daisy", "Christopher", "Concepcion", "Dominador", "Soledad", "Isagani", "Nieves", "Wilfredo", "Esperanza"
    ];
    private static readonly string[] MiddleNames =
    [
        "Cruz", "Santos", "Reyes", "Garcia", "Mendoza", "Aquino", "Dela Cruz", "Gutierrez", "Rivera", "Torres"
    ];
    private static readonly string[] LastNames =
    [
        "Dela Cruz", "Santos", "Reyes", "Garcia", "Mendoza", "Aquino", "Gutierrez", "Rivera", "Torres", "Fernandez",
        "Gonzales", "Lopez", "Martinez", "Rodriguez", "Villanueva", "Castillo", "Cruz", "Diaz", "Flores", "Gomez",
        "Hernandez", "Jimenez", "Lim", "Manalo", "Navarro", "Ocampo", "Padilla", "Quijano", "Ramirez", "Santiago",
        "Tan", "Uy", "Valdez", "Wong", "Ybanez", "Zamora", "Alcantara", "Bautista", "Contreras", "De Leon",
        "Enriquez", "Francisco", "Galang", "Hizon", "Infante", "Jacinto", "King", "Lansang", "Malonzo", "Ong",
        "Pascual", "Quinto", "Roque", "Salonga", "Tolentino", "Ubaldo", "Vargas", "Yap", "Ang", "Belen",
        "Cordero", "Dimaano", "Esteban", "Fajardo", "Guevarra", "Herrera", "Ignacio", "Luciano", "Mallari", "Natividad",
        "Ordona", "Palma", "Quiambao", "Ramos", "Samson", "Tecson", "Umali", "Viray", "Zulueta", "Aguilar",
        "Burgos", "Cabrera", "David", "Espiritu", "Go", "Hidalgo", "Lazaro", "Miranda", "Nigro", "Pineda"
    ];
    private static readonly string[] Suffixes = ["Jr.", "Sr.", "III", "IV", null!, null!, null!];
    private static readonly string[] Genders = ["Male", "Female"];
    private static readonly string[] CivilStatuses = ["Single", "Married", "Divorced", "Widowed"];
    private static readonly string[] Religions = ["Roman Catholic", "Muslim", "Iglesia ni Cristo", "Evangelical", "Buddhist", null!];
    private static readonly string[] EducationalAttainments =
    [
        "High School Graduate", "College Undergraduate", "College Graduate", "Master's Degree", "Doctorate Degree", "Vocational"
    ];
    private static readonly string[] Nationalities = ["Filipino", "Filipino", "Filipino", "Filipino", "Filipino", "Filipino", "Dual Citizen"];
    private static readonly string[] Countries = ["Philippines", "Philippines", "Philippines", "Philippines", "Philippines", "USA", "Canada", "Australia"];
    private static readonly string[] Cities =
    [
        "Manila", "Quezon City", "Makati", "Taguig", "Pasig", "Mandaluyong", "Parañaque", "Cebu City", "Davao City", "Baguio",
        "Caloocan", "Las Piñas", "Malabon", "Navotas", "Muntinlupa", "Pasay", "Valenzuela", "Marikina", "San Juan", "Angeles"
    ];
    private static readonly string[] Provinces =
    [
        "Metro Manila", "Cebu", "Davao del Sur", "Benguet", "Laguna", "Cavite", "Rizal", "Bulacan", "Pampanga", "Batangas",
        "Nueva Ecija", "Pangasinan", "Ilocos Norte", "Ilocos Sur", "Iloilo", "Negros Occidental", "Zamboanga", "Bukidnon"
    ];
    private static readonly string[] Barangays =
    [
        "Barangay 1", "Barangay 2", "Poblacion", "San Roque", "San Jose", "Sto. Niño", "Bagong Silangan", "Ugong", "Addition Hills", "West Rembo",
        "East Rembo", "Pinagkaisa", "San Antonio", "San Isidro", "Santa Cruz", "Santa Lucia", "Santa Maria", "Santiago", "Santo Tomas", "Malate"
    ];
    private static readonly string[] Streets =
    [
        "Rizal Ave.", "Mabini St.", "Luna St.", "Bonifacio St.", "Aguinaldo Hwy", "Taft Ave.", "Commonwealth Ave.", "EDSA", "Quezon Ave.", "Katipunan Ave.",
        "Shaw Blvd.", "Ortigas Ave.", "Ayala Ave.", "Paseo de Roxas", "Makati Ave.", "Tomas Morato", "Timog Ave.", "West Ave.", "East Ave.", "Circumferential Road"
    ];
    private static readonly string[] SubdivisionPurok = [null!, null!, null!, "Phase 1", "Phase 2", "Unit 3", "Block 5", "Lot 12", null!, null!];
    private static readonly string[] OwnerOrLessee = ["Owner", "Lessee", "Owner", "Owner", "Lessee"];
    private static readonly string[] EmployeeLevels =
    [
        "Rank-and-File", "Rank-and-File", "Supervisor", "Supervisor", "Manager", "Manager", "Director", "Executive"
    ];
    private static readonly string[] Companies =
    [
        "OPTODEV Corporation", "OPTODEV Solutions", "OPTODEV Technologies", "OPTODEV Services", "OPTODEV International",
        "OPTODEV Philippines", "OPTODEV Global", "OPTODEV Systems", "OPTODEV Innovations", "OPTODEV Consulting",
        "Philippine Telecom Inc.", "Metro Bank Corp.", "SM Investments", "Ayala Corporation", "Jollibee Foods"
    ];
    private static readonly string[] Occupations =
    [
        "Software Engineer", "Systems Analyst", "Project Manager", "HR Coordinator", "Financial Analyst", "Accountant",
        "Network Administrator", "Database Administrator", "IT Support Specialist", "Business Analyst",
        "Marketing Specialist", "Sales Representative", "Operations Manager", "Quality Assurance Analyst", "UX Designer",
        "Data Scientist", "DevOps Engineer", "Security Analyst", "Technical Writer", "Product Owner"
    ];
    private static readonly string[] IncomePeriods = ["Monthly", "Semi-Monthly", "Weekly", "Daily"];
    private static readonly string[] IdTypes =
    [
        "Passport", "Driver's License", "UMID", "PRC ID", "Postal ID", "Voter's ID", "National ID"
    ];
    private static readonly string[] IdCountries =
    [
        "Philippines", "Philippines", "Philippines", "Philippines", "Philippines", "USA", "Japan", "Singapore"
    ];
    private static readonly string[] RelationshipTypes =
    [
        "Spouse", "Parent", "Sibling", "Child", "Friend", "Colleague"
    ];

    public static async Task SeedAsync(MembersDbContext context)
    {
        if (await context.Set<Member>().AnyAsync())
            return;

        var members = new List<Member>(1000);
        var emailCounter = 0;

        for (var i = 0; i < 1000; i++)
        {
            var firstName = Pick(FirstNames);
            var lastName = Pick(LastNames);
            var middleName = Chance(0.7) ? Pick(MiddleNames) : null;
            var title = Pick(Titles);
            var suffix = Chance(0.2) ? Pick(Suffixes.Where(s => s is not null).ToArray()) : null;
            var alias = Chance(0.15) ? $"{firstName[..Math.Min(3, firstName.Length)]}{i % 100}" : null;

            var gender = Pick(Genders);
            var civilStatus = GetCivilStatus(i);
            var religion = Pick(Religions);
            var education = Pick(EducationalAttainments);
            var nationality = Pick(Nationalities);
            var countryOfBirth = Pick(Countries);
            var placeOfBirth = $"{Pick(Cities)}, {Pick(Provinces)}";
            var dateOfBirth = RandomDate(1950, 2000, 1, 1, 28);

            var emailCounterVal = Interlocked.Increment(ref emailCounter);
            var email = $"{firstName.ToLowerInvariant()}.{lastName.ToLowerInvariant().Replace(" ", "")}{emailCounterVal}@email.com";
            var contactNumber = $"+639{Rng.Next(10_000_000, 99_999_999)}";

            var tin = $"{Rng.Next(100, 999)}-{Rng.Next(100, 999)}-{Rng.Next(100, 999)}-{Rng.Next(10000, 99999)}";
            var sss = $"{Rng.Next(10, 99)}-{Rng.Next(1000000, 9999999)}-{Rng.Next(1, 9)}";

            var spouse = civilStatus == "Married"
                ? new SpouseInfo(Pick(FirstNames), Chance(0.5) ? Pick(MiddleNames) : null, Pick(LastNames))
                : null;
            var motherMaidenName = Chance(0.9) ? $"{Pick(FirstNames)} {Pick(LastNames)}" : null;
            var father = Chance(0.8)
                ? new FatherInfo(Pick(FirstNames), Chance(0.5) ? Pick(MiddleNames) : null, Pick(LastNames), Chance(0.1) ? Pick(Suffixes.Where(s => s is not null).ToArray()) : null)
                : null;

            var idType = Pick(IdTypes);
            var idIssueDate = RandomDate(2015, 2025, 1, 1, 28);
            var idExpiryDate = idIssueDate.AddYears(Rng.Next(3, 6));

            var city = Pick(Cities);
            var province = Pick(Provinces);
            var barangay = Pick(Barangays);
            var street = Pick(Streets);
            var subdiv = Pick(SubdivisionPurok);
            var postalCode = $"{Rng.Next(1000, 9999)}";
            var country = "Philippines";
            var currentAddress = new Address(
                $"{Rng.Next(1, 9999)} {street}",
                city, postalCode, barangay, subdiv, province, country,
                Pick(OwnerOrLessee), RandomDate(2018, 2025, 1, 1, 28));

            var sameAsCurrent = Chance(0.7);
            var permAddr = sameAsCurrent
                ? new PermanentAddressInfo(true, null)
                : new PermanentAddressInfo(false, new Address(
                    $"{Rng.Next(1, 9999)} {Pick(Streets)}",
                    Pick(Cities), $"{Rng.Next(1000, 9999)}", Pick(Barangays), Pick(SubdivisionPurok),
                    Pick(Provinces), "Philippines", Pick(OwnerOrLessee), RandomDate(2018, 2025, 1, 1, 28)));

            var occupation = Pick(Occupations);
            var company = Pick(Companies);

            var member = Member.Create(
                new PersonName(title, firstName, middleName, lastName, suffix, alias),
                new Demographics(dateOfBirth, placeOfBirth, countryOfBirth, nationality,
                    gender, civilStatus, religion, education),
                new ContactDetails(email, contactNumber),
                new DependentInfo(Rng.Next(0, 6)),
                new RelatedPersons(spouse, motherMaidenName, father),
                new GovernmentIds(tin, sss),
                new PrimaryIdentification(idType, $"{idType}-{Rng.Next(100000, 999999)}", idIssueDate, idExpiryDate, Pick(IdCountries)),
                currentAddress,
                permAddr,
                new EmergencyContact(
                    $"{Pick(FirstNames)} {Pick(LastNames)}",
                    Pick(RelationshipTypes),
                    $"+639{Rng.Next(10_000_000, 99_999_999)}"),
                new EmploymentDetails(
                    Pick(EmployeeLevels), company, $"EMP-{Rng.Next(10000, 99999)}",
                    (decimal)(Rng.Next(15000, 250000) / 100.0) * 100,
                    Pick(IncomePeriods), occupation,
                    RandomDate(2010, 2025, 1, 1, 28),
                    Chance(0.3) ? RandomDate(2023, 2025, 1, 1, 28) : null),
                new Consent(true, true, $"{firstName} {lastName}", DateTime.UtcNow));

            if (Chance(0.2))
                member.UpdateStatus(MemberStatus.UnderReview);
            else if (Chance(0.1))
                member.UpdateStatus(MemberStatus.Approved);
            else if (Chance(0.05))
                member.UpdateStatus(MemberStatus.Rejected);

            members.Add(member);
        }

        context.Set<Member>().AddRange(members);
        await context.SaveChangesAsync();
    }

    private static string Pick(string[] items) =>
        items[Rng.Next(items.Length)];

    private static bool Chance(double probability) =>
        Rng.NextDouble() < probability;

    public static async Task SeedAdminUsersAsync(MembersDbContext context, Application.Common.IPasswordHasher passwordHasher)
    {
        if (await context.AdminUsers.AnyAsync())
            return;

        var admins = new List<AdminUser>
        {
            new("admin@optodev.com", passwordHasher.Hash("Admin123!"), "Admin"),
            new("hradmin@optodev.com", passwordHasher.Hash("HRAdmin123!"), "HRAdmin"),
        };

        context.AdminUsers.AddRange(admins);
        await context.SaveChangesAsync();
    }

    private static string GetCivilStatus(int index)
    {
        if (index < 400) return "Single";
        if (index < 800) return "Married";
        return index < 950 ? "Divorced" : "Widowed";
    }

    private static DateTime RandomDate(int fromYear, int toYear, int minDay, int minMonth, int maxDay) =>
        DateTime.SpecifyKind(
            new(Rng.Next(fromYear, toYear), Rng.Next(1, 13), Rng.Next(minDay, maxDay + 1)),
            DateTimeKind.Utc);
}
