using LibraryManagement.Domain.Common;

namespace LibraryManagement.Domain.Entities;

public class Publisher : AuditableEntity
{
    private readonly List<Book> _books = new();

    public string Name { get; private set; } = string.Empty;
    public string? Website { get; private set; }
    public string? ContactEmail { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string? Address { get; private set; }
    public bool IsActive { get; private set; }

    public IReadOnlyCollection<Book> Books => _books.AsReadOnly();

    private Publisher() { }

    public static Publisher Create(
        string name,
        string? website = null,
        string? contactEmail = null,
        string? phoneNumber = null,
        string? address = null,
        Guid? id = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Publisher name cannot be empty.", nameof(name));

        return new Publisher
        {
            Id = id ?? Guid.NewGuid(),
            Name = name.Trim(),
            Website = website?.Trim(),
            ContactEmail = contactEmail?.Trim().ToLowerInvariant(),
            PhoneNumber = phoneNumber?.Trim(),
            Address = address?.Trim(),
            IsActive = true
        };
    }

    public void Update(
        string name,
        string? website = null,
        string? contactEmail = null,
        string? phoneNumber = null,
        string? address = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Publisher name cannot be empty.", nameof(name));

        Name = name.Trim();
        Website = website?.Trim();
        ContactEmail = contactEmail?.Trim().ToLowerInvariant();
        PhoneNumber = phoneNumber?.Trim();
        Address = address?.Trim();
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
