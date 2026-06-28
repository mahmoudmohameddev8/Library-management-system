using LibraryManagement.Domain.Common;

namespace LibraryManagement.Domain.Entities;

public class Language : AuditableEntity
{
    private readonly List<Book> _books = new();

    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;

    public IReadOnlyCollection<Book> Books => _books.AsReadOnly();

    private Language() { }

    public static Language Create(string name, string code, Guid? id = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Language name cannot be empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Language code cannot be empty.", nameof(code));

        return new Language
        {
            Id = id ?? Guid.NewGuid(),
            Name = name.Trim(),
            Code = code.Trim().ToUpperInvariant()
        };
    }

    public void Update(string name, string code)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Language name cannot be empty.", nameof(name));

        Name = name.Trim();
        Code = code.Trim().ToUpperInvariant();
    }
}
