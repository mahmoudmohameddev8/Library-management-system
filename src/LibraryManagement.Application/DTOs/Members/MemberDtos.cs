namespace LibraryManagement.Application.DTOs.Members;

public record MemberDto(
    Guid Id,
    string MembershipNumber,
    string FirstName,
    string LastName,
    string FullName,
    string Email,
    string? PhoneNumber,
    string? Address,
    string Status,
    DateTime MembershipStartDate,
    DateTime MembershipExpiryDate,
    int MaxBorrowLimit
);

public record CreateMemberRequest(
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber = null,
    string? Address = null,
    int MembershipDurationMonths = 12,
    int MaxBorrowLimit = 5
);

public record UpdateMemberRequest(
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber = null,
    string? Address = null
);

public record RenewMembershipRequest(int Months = 12);
