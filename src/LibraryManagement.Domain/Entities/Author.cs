using LibraryManagement.Domain.Common;

namespace LibraryManagement.Domain.Entities;

public class Author : AuditableEntity
{
    private readonly List<BookAuthor> _bookAuthors = new();

    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string? Biography { get; private set; }
    public string? Email { get; private set; }
    public string? Website { get; private set; }
    public DateOnly? BirthDate { get; private set; }
    public string? Nationality { get; private set; }

    public string FullName => $"{FirstName} {LastName}";

    public IReadOnlyCollection<BookAuthor> BookAuthors => _bookAuthors.AsReadOnly();

    private Author() { }

    public static Author Create(
        string firstName,
        string lastName,
        string? biography = null,
        string? email = null,
        string? website = null,
        DateOnly? birthDate = null,
        string? nationality = null,
        Guid? id = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty.", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty.", nameof(lastName));

        return new Author
        {
            Id = id ?? Guid.NewGuid(),
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Biography = biography?.Trim(),
            Email = email?.Trim().ToLowerInvariant(),
            Website = website?.Trim(),
            BirthDate = birthDate,
            Nationality = nationality?.Trim()
        };
    }

    public void Update(
        string firstName,
        string lastName,
        string? biography = null,
        string? email = null,
        string? website = null,
        DateOnly? birthDate = null,
        string? nationality = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty.", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty.", nameof(lastName));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Biography = biography?.Trim();
        Email = email?.Trim().ToLowerInvariant();
        Website = website?.Trim();
        BirthDate = birthDate;
        Nationality = nationality?.Trim();
    }
}
