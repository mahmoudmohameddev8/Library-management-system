using LibraryManagement.Domain.Common;
using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Domain.Entities;

public class Member : AuditableEntity
{
    private readonly List<BorrowingTransaction> _borrowingTransactions = new();

    public string MembershipNumber { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? PhoneNumber { get; private set; }
    public string? Address { get; private set; }
    public DateTime MembershipStartDate { get; private set; }
    public DateTime MembershipExpiryDate { get; private set; }
    public MembershipStatus Status { get; private set; }
    public int MaxBorrowLimit { get; private set; }

    public string FullName => $"{FirstName} {LastName}";

    public IReadOnlyCollection<BorrowingTransaction> BorrowingTransactions => _borrowingTransactions.AsReadOnly();

    private Member() { }

    public static Member Create(
        string membershipNumber,
        string firstName,
        string lastName,
        string email,
        string? phoneNumber = null,
        string? address = null,
        int membershipDurationMonths = 12,
        int maxBorrowLimit = 5,
        Guid? id = null)
    {
        if (string.IsNullOrWhiteSpace(membershipNumber))
            throw new ArgumentException("Membership number cannot be empty.", nameof(membershipNumber));
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty.", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty.", nameof(lastName));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.", nameof(email));

        return new Member
        {
            Id = id ?? Guid.NewGuid(),
            MembershipNumber = membershipNumber.Trim(),
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            PhoneNumber = phoneNumber?.Trim(),
            Address = address?.Trim(),
            MembershipStartDate = DateTime.UtcNow,
            MembershipExpiryDate = DateTime.UtcNow.AddMonths(membershipDurationMonths),
            Status = MembershipStatus.Active,
            MaxBorrowLimit = maxBorrowLimit
        };
    }

    public void Update(string firstName, string lastName, string email, string? phoneNumber = null, string? address = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty.", nameof(firstName));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.", nameof(email));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email.Trim().ToLowerInvariant();
        PhoneNumber = phoneNumber?.Trim();
        Address = address?.Trim();
    }

    public void RenewMembership(int months = 12)
    {
        var baseDate = MembershipExpiryDate > DateTime.UtcNow ? MembershipExpiryDate : DateTime.UtcNow;
        MembershipExpiryDate = baseDate.AddMonths(months);
        Status = MembershipStatus.Active;
    }

    public bool CanBorrow(int currentlyBorrowed)
    {
        return Status == MembershipStatus.Active
               && MembershipExpiryDate > DateTime.UtcNow
               && currentlyBorrowed < MaxBorrowLimit;
    }

    public void Suspend() => Status = MembershipStatus.Suspended;
    public void Activate() => Status = MembershipStatus.Active;
    public void Expire() => Status = MembershipStatus.Expired;
}
