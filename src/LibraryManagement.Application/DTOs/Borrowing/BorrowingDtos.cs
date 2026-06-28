namespace LibraryManagement.Application.DTOs.Borrowing;

public record BorrowingTransactionDto(
    Guid Id,
    Guid BookId,
    string BookTitle,
    string BookISBN,
    Guid MemberId,
    string MemberFullName,
    string MembershipNumber,
    Guid ProcessedByUserId,
    string ProcessedByUsername,
    DateTime BorrowDate,
    DateTime DueDate,
    DateTime? ReturnDate,
    string Status,
    bool IsOverdue,
    string? Notes
);

public record BorrowBookRequest(
    Guid BookId,
    Guid MemberId,
    int BorrowDurationDays = 14,
    string? Notes = null
);

public record ReturnBookRequest(string? Notes = null);
