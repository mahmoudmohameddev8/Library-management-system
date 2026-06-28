using LibraryManagement.Domain.Common;

namespace LibraryManagement.Domain.Entities;

public class User : AuditableEntity
{
    private readonly List<BorrowingTransaction> _processedTransactions = new();

    public string Username { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public Guid RoleId { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    public Role Role { get; private set; } = null!;
    public IReadOnlyCollection<BorrowingTransaction> ProcessedTransactions => _processedTransactions.AsReadOnly();

    public string FullName => $"{FirstName} {LastName}";

    private User() { }

    public static User Create(
        string username,
        string email,
        string passwordHash,
        string firstName,
        string lastName,
        Guid roleId,
        Guid? id = null)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty.", nameof(username));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.", nameof(email));
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty.", nameof(passwordHash));

        return new User
        {
            Id = id ?? Guid.NewGuid(),
            Username = username.Trim().ToLowerInvariant(),
            Email = email.Trim().ToLowerInvariant(),
            PasswordHash = passwordHash,
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            RoleId = roleId,
            IsActive = true
        };
    }

    public void UpdateProfile(string firstName, string lastName, string email)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty.", nameof(firstName));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.", nameof(email));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email.Trim().ToLowerInvariant();
    }

    public void ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("Password hash cannot be empty.", nameof(newPasswordHash));

        PasswordHash = newPasswordHash;
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    public void ChangeRole(Guid newRoleId)
    {
        RoleId = newRoleId;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
