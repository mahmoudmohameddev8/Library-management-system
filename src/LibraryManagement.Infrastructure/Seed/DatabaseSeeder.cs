using BCrypt.Net;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LibraryManagement.Infrastructure.Seed;

public static class DatabaseSeeder
{
    // Fixed seed GUIDs for deterministic, idempotent seeding
    private static readonly Guid EnglishId = new("10000000-0000-0000-0000-000000000001");
    private static readonly Guid ArabicId = new("10000000-0000-0000-0000-000000000002");
    private static readonly Guid FrenchId = new("10000000-0000-0000-0000-000000000003");
    private static readonly Guid SpanishId = new("10000000-0000-0000-0000-000000000004");
    private static readonly Guid GermanId = new("10000000-0000-0000-0000-000000000005");

    private static readonly Guid PearsonId = new("20000000-0000-0000-0000-000000000001");
    private static readonly Guid OReillyId = new("20000000-0000-0000-0000-000000000002");
    private static readonly Guid MicrosoftPressId = new("20000000-0000-0000-0000-000000000003");
    private static readonly Guid ManningId = new("20000000-0000-0000-0000-000000000004");
    private static readonly Guid AddisonWesleyId = new("20000000-0000-0000-0000-000000000005");

    private static readonly Guid FictionId = new("30000000-0000-0000-0000-000000000001");
    private static readonly Guid MysteryId = new("30000000-0000-0000-0000-000000000002");
    private static readonly Guid ThrillerCatId = new("30000000-0000-0000-0000-000000000003");
    private static readonly Guid SciFiId = new("30000000-0000-0000-0000-000000000004");
    private static readonly Guid NonFictionId = new("30000000-0000-0000-0000-000000000005");
    private static readonly Guid TechnologyId = new("30000000-0000-0000-0000-000000000006");
    private static readonly Guid ProgrammingId = new("30000000-0000-0000-0000-000000000007");
    private static readonly Guid WebDevId = new("30000000-0000-0000-0000-000000000008");
    private static readonly Guid ScienceId = new("30000000-0000-0000-0000-000000000009");
    private static readonly Guid BiologyId = new("30000000-0000-0000-0000-000000000010");
    private static readonly Guid PhysicsId = new("30000000-0000-0000-0000-000000000011");

    private static readonly Guid Author1Id = new("40000000-0000-0000-0000-000000000001");
    private static readonly Guid Author2Id = new("40000000-0000-0000-0000-000000000002");
    private static readonly Guid Author3Id = new("40000000-0000-0000-0000-000000000003");
    private static readonly Guid Author4Id = new("40000000-0000-0000-0000-000000000004");
    private static readonly Guid Author5Id = new("40000000-0000-0000-0000-000000000005");

    private static readonly Guid Book1Id = new("50000000-0000-0000-0000-000000000001");
    private static readonly Guid Book2Id = new("50000000-0000-0000-0000-000000000002");
    private static readonly Guid Book3Id = new("50000000-0000-0000-0000-000000000003");
    private static readonly Guid Book4Id = new("50000000-0000-0000-0000-000000000004");
    private static readonly Guid Book5Id = new("50000000-0000-0000-0000-000000000005");

    private static readonly Guid AdminUserId = new("60000000-0000-0000-0000-000000000001");
    private static readonly Guid LibrarianUserId = new("60000000-0000-0000-0000-000000000002");
    private static readonly Guid StaffUserId = new("60000000-0000-0000-0000-000000000003");

    public static async Task SeedAsync(LibraryDbContext context, ILogger logger)
    {
        await SeedRolesAsync(context, logger);
        await SeedUsersAsync(context, logger);
        await SeedLanguagesAsync(context, logger);
        await SeedPublishersAsync(context, logger);
        await SeedCategoriesAsync(context, logger);
        await SeedAuthorsAsync(context, logger);
        await SeedBooksAsync(context, logger);
        await SeedMembersAsync(context, logger);
    }

    private static async Task SeedRolesAsync(LibraryDbContext context, ILogger logger)
    {
        if (await context.Roles.AnyAsync())
        {
            logger.LogInformation("Roles already seeded — skipping.");
            return;
        }

        var roles = new[]
        {
            Role.Create(Role.AdminRoleId,     Role.Admin,     "Full system access with all administrative privileges"),
            Role.Create(Role.LibrarianRoleId, Role.Librarian, "Manages books, members, and borrowing operations"),
            Role.Create(Role.StaffRoleId,     Role.Staff,     "Handles day-to-day borrowing and returning operations")
        };

        await context.Roles.AddRangeAsync(roles);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} roles.", roles.Length);
    }

    private static async Task SeedUsersAsync(LibraryDbContext context, ILogger logger)
    {
        if (await context.Users.AnyAsync())
        {
            logger.LogInformation("Users already seeded — skipping.");
            return;
        }

        var now = DateTime.UtcNow;

        var users = new[]
        {
            CreateUser(AdminUserId,     "admin",     "admin@library.com",     BCrypt.Net.BCrypt.HashPassword("Admin@123!"),     "System", "Administrator", Role.AdminRoleId,     now),
            CreateUser(LibrarianUserId, "librarian", "librarian@library.com", BCrypt.Net.BCrypt.HashPassword("Librarian@123!"), "Jane",   "Smith",         Role.LibrarianRoleId, now),
            CreateUser(StaffUserId,     "staff",     "staff@library.com",     BCrypt.Net.BCrypt.HashPassword("Staff@123!"),     "John",   "Doe",           Role.StaffRoleId,     now)
        };

        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} users.", users.Length);
    }

    private static async Task SeedLanguagesAsync(LibraryDbContext context, ILogger logger)
    {
        if (await context.Languages.AnyAsync())
        {
            logger.LogInformation("Languages already seeded — skipping.");
            return;
        }

        var now = DateTime.UtcNow;

        var languages = new[]
        {
            CreateLanguage(EnglishId, "English", "EN", now),
            CreateLanguage(ArabicId,  "Arabic",  "AR", now),
            CreateLanguage(FrenchId,  "French",  "FR", now),
            CreateLanguage(SpanishId, "Spanish", "ES", now),
            CreateLanguage(GermanId,  "German",  "DE", now)
        };

        await context.Languages.AddRangeAsync(languages);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} languages.", languages.Length);
    }

    private static async Task SeedPublishersAsync(LibraryDbContext context, ILogger logger)
    {
        if (await context.Publishers.AnyAsync())
        {
            logger.LogInformation("Publishers already seeded — skipping.");
            return;
        }

        var now = DateTime.UtcNow;

        var publishers = new[]
        {
            CreatePublisher(PearsonId,        "Pearson Education",   "https://www.pearson.com",    now),
            CreatePublisher(OReillyId,         "O'Reilly Media",      "https://www.oreilly.com",    now),
            CreatePublisher(MicrosoftPressId, "Microsoft Press",     "https://www.microsoftpress.com", now),
            CreatePublisher(ManningId,        "Manning Publications", "https://www.manning.com",   now),
            CreatePublisher(AddisonWesleyId,  "Addison-Wesley",      "https://www.pearson.com",    now)
        };

        await context.Publishers.AddRangeAsync(publishers);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} publishers.", publishers.Length);
    }

    private static async Task SeedCategoriesAsync(LibraryDbContext context, ILogger logger)
    {
        if (await context.Categories.AnyAsync())
        {
            logger.LogInformation("Categories already seeded — skipping.");
            return;
        }

        var now = DateTime.UtcNow;

        // Root categories
        var rootCategories = new[]
        {
            CreateCategory(FictionId,    "Fiction",     "Fictional literature and stories",               null,         now),
            CreateCategory(NonFictionId, "Non-Fiction", "Factual and informational books",                null,         now),
            CreateCategory(ScienceId,    "Science",     "Scientific books across disciplines",            null,         now)
        };

        await context.Categories.AddRangeAsync(rootCategories);
        await context.SaveChangesAsync();

        // Child categories of Fiction
        var fictionChildren = new[]
        {
            CreateCategory(MysteryId,    "Mystery",         "Detective and mystery novels",                  FictionId, now),
            CreateCategory(ThrillerCatId,"Thriller",        "Suspense and thriller novels",                  FictionId, now),
            CreateCategory(SciFiId,      "Science Fiction", "Futuristic and speculative fiction",            FictionId, now)
        };

        await context.Categories.AddRangeAsync(fictionChildren);
        await context.SaveChangesAsync();

        // Child category of Non-Fiction
        var nonFictionChildren = new[]
        {
            CreateCategory(TechnologyId, "Technology", "Technology and computing books", NonFictionId, now)
        };

        await context.Categories.AddRangeAsync(nonFictionChildren);
        await context.SaveChangesAsync();

        // Grandchild categories under Technology
        var techChildren = new[]
        {
            CreateCategory(ProgrammingId, "Programming",    "Programming languages and paradigms", TechnologyId, now),
            CreateCategory(WebDevId,      "Web Development","Web technologies and frameworks",    TechnologyId, now)
        };

        await context.Categories.AddRangeAsync(techChildren);
        await context.SaveChangesAsync();

        // Children of Science
        var scienceChildren = new[]
        {
            CreateCategory(BiologyId, "Biology", "Life sciences", ScienceId, now),
            CreateCategory(PhysicsId, "Physics", "Physical sciences and mechanics", ScienceId, now)
        };

        await context.Categories.AddRangeAsync(scienceChildren);
        await context.SaveChangesAsync();

        logger.LogInformation("Seeded hierarchical categories.");
    }

    private static async Task SeedAuthorsAsync(LibraryDbContext context, ILogger logger)
    {
        if (await context.Authors.AnyAsync())
        {
            logger.LogInformation("Authors already seeded — skipping.");
            return;
        }

        var now = DateTime.UtcNow;

        var authors = new[]
        {
            CreateAuthor(Author1Id, "Robert",   "Martin",    "The Clean Code author and Agile manifesto contributor.", "Uncle Bob", now),
            CreateAuthor(Author2Id, "Martin",   "Fowler",    "Author of Refactoring and Patterns of Enterprise Application Architecture.", "Martin Fowler", now),
            CreateAuthor(Author3Id, "Eric",     "Evans",     "Creator of Domain-Driven Design methodology.", "Eric Evans", now),
            CreateAuthor(Author4Id, "Andrew",   "Hunt",      "The Pragmatic Programmer co-author.", "Andy Hunt", now),
            CreateAuthor(Author5Id, "Agatha",   "Christie",  "Queen of Crime and bestselling mystery novelist.", "Agatha Christie", now)
        };

        await context.Authors.AddRangeAsync(authors);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} authors.", authors.Length);
    }

    private static async Task SeedBooksAsync(LibraryDbContext context, ILogger logger)
    {
        if (await context.Books.AnyAsync())
        {
            logger.LogInformation("Books already seeded — skipping.");
            return;
        }

        var now = DateTime.UtcNow;

        var books = new[]
        {
            CreateBook(Book1Id, "9780132350884", "Clean Code: A Handbook of Agile Software Craftsmanship",
                "A guide to writing clean, readable, and maintainable code.", new DateOnly(2008, 8, 1),
                5, AddisonWesleyId, EnglishId, ProgrammingId, 431, now),

            CreateBook(Book2Id, "9780201485677", "Refactoring: Improving the Design of Existing Code",
                "Systematic techniques for improving the structure of existing code.", new DateOnly(1999, 6, 28),
                3, AddisonWesleyId, EnglishId, ProgrammingId, 448, now),

            CreateBook(Book3Id, "9780321125217", "Domain-Driven Design: Tackling Complexity in the Heart of Software",
                "A methodology for developing complex software by connecting the implementation to an evolving model.", new DateOnly(2003, 8, 20),
                4, AddisonWesleyId, EnglishId, ProgrammingId, 560, now),

            CreateBook(Book4Id, "9780135957059", "The Pragmatic Programmer: Your Journey to Mastery",
                "Practical advice for software developers from code to career.", new DateOnly(2019, 9, 13),
                6, AddisonWesleyId, EnglishId, ProgrammingId, 352, now),

            CreateBook(Book5Id, "9780007527526", "And Then There Were None",
                "Ten people lured to a remote island by a mysterious host, each accused of past crimes.", new DateOnly(1939, 11, 6),
                8, PearsonId, EnglishId, MysteryId, 272, now)
        };

        await context.Books.AddRangeAsync(books);
        await context.SaveChangesAsync();

        // Seed book-author relationships (many-to-many)
        var bookAuthors = new[]
        {
            BookAuthor.Create(Book1Id, Author1Id, isPrimaryAuthor: true),
            BookAuthor.Create(Book2Id, Author2Id, isPrimaryAuthor: true),
            BookAuthor.Create(Book3Id, Author3Id, isPrimaryAuthor: true),
            BookAuthor.Create(Book4Id, Author4Id, isPrimaryAuthor: true),
            BookAuthor.Create(Book5Id, Author5Id, isPrimaryAuthor: true)
        };

        await context.BookAuthors.AddRangeAsync(bookAuthors);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} books with author relationships.", books.Length);
    }

    private static async Task SeedMembersAsync(LibraryDbContext context, ILogger logger)
    {
        if (await context.Members.AnyAsync())
        {
            logger.LogInformation("Members already seeded — skipping.");
            return;
        }

        var now = DateTime.UtcNow;

        var members = new[]
        {
            CreateMember("LIB-000001", "Alice",   "Johnson", "alice.johnson@email.com",  "+1-555-0101", "123 Main St, Springfield", now),
            CreateMember("LIB-000002", "Bob",     "Williams","bob.williams@email.com",   "+1-555-0102", "456 Oak Ave, Shelbyville", now),
            CreateMember("LIB-000003", "Carol",   "Davis",   "carol.davis@email.com",    "+1-555-0103", "789 Pine Rd, Ogdenville",  now),
            CreateMember("LIB-000004", "David",   "Brown",   "david.brown@email.com",    "+1-555-0104", "101 Elm St, North Haverbrook", now),
            CreateMember("LIB-000005", "Emily",   "Wilson",  "emily.wilson@email.com",   "+1-555-0105", "202 Birch Ln, Capital City", now)
        };

        await context.Members.AddRangeAsync(members);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} members.", members.Length);
    }

    // ─── Private factory helpers ──────────────────────────────────────────────

    private static User CreateUser(Guid id, string username, string email, string passwordHash,
        string firstName, string lastName, Guid roleId, DateTime now)
    {
        var user = User.Create(username, email, passwordHash, firstName, lastName, roleId, id: id);
        user.CreatedAt = now;
        user.CreatedBy = "Seed";
        return user;
    }

    private static Language CreateLanguage(Guid id, string name, string code, DateTime now)
    {
        var lang = Language.Create(name, code, id: id);
        lang.CreatedAt = now;
        lang.CreatedBy = "Seed";
        return lang;
    }

    private static Publisher CreatePublisher(Guid id, string name, string website, DateTime now)
    {
        var pub = Publisher.Create(name, website, id: id);
        pub.CreatedAt = now;
        pub.CreatedBy = "Seed";
        return pub;
    }

    private static Category CreateCategory(Guid id, string name, string description,
        Guid? parentId, DateTime now)
    {
        var cat = Category.Create(name, description, parentId, id: id);
        cat.CreatedAt = now;
        cat.CreatedBy = "Seed";
        return cat;
    }

    private static Author CreateAuthor(Guid id, string firstName, string lastName,
        string bio, string website, DateTime now)
    {
        var author = Author.Create(firstName, lastName, biography: bio, website: website, id: id);
        author.CreatedAt = now;
        author.CreatedBy = "Seed";
        return author;
    }

    private static Book CreateBook(Guid id, string isbn, string title, string description,
        DateOnly publishedDate, int totalCopies, Guid publisherId, Guid languageId,
        Guid categoryId, int pageCount, DateTime now)
    {
        var book = Book.Create(isbn, title, description, publishedDate, totalCopies,
                               publisherId, languageId, categoryId, pageCount: pageCount, id: id);
        book.CreatedAt = now;
        book.CreatedBy = "Seed";
        return book;
    }

    private static Member CreateMember(string membershipNumber, string firstName, string lastName,
        string email, string phone, string address, DateTime now)
    {
        var member = Member.Create(membershipNumber, firstName, lastName, email, phone, address);
        member.CreatedAt = now;
        member.CreatedBy = "Seed";
        return member;
    }
}
