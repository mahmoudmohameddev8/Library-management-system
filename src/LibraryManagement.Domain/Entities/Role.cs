using LibraryManagement.Domain.Common;

namespace LibraryManagement.Domain.Entities;

public class Role : BaseEntity
{
    private readonly List<User> _users = new();

    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    public IReadOnlyCollection<User> Users => _users.AsReadOnly();

    public const string Admin = "Admin";
    public const string Librarian = "Librarian";
    public const string Staff = "Staff";

    public static readonly Guid AdminRoleId = new("00000000-0000-0000-0000-000000000001");
    public static readonly Guid LibrarianRoleId = new("00000000-0000-0000-0000-000000000002");
    public static readonly Guid StaffRoleId = new("00000000-0000-0000-0000-000000000003");

    private Role() { }

    public static Role Create(Guid id, string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name cannot be empty.", nameof(name));

        return new Role
        {
            Id = id,
            Name = name.Trim(),
            Description = description.Trim()
        };
    }
}
