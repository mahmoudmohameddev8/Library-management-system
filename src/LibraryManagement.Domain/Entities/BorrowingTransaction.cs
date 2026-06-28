using LibraryManagement.Domain.Common;
using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Domain.Entities;

public class BorrowingTransaction : AuditableEntity
{
    public Guid BookId { get; private set; }
    public Guid MemberId { get; private set; }
    public Guid ProcessedByUserId { get; private set; }
    public Guid? ReturnProcessedByUserId { get; private set; }
    public DateTime BorrowDate { get; private set; }
    public DateTime DueDate { get; private set; }
    public DateTime? ReturnDate { get; private set; }
    public BorrowingStatus Status { get; private set; }
    public string? Notes { get; private set; }

    public Book Book { get; private set; } = null!;
    public Member Member { get; private set; } = null!;
    public User ProcessedByUser { get; private set; } = null!;
    public User? ReturnProcessedByUser { get; private set; }

    public bool IsOverdue => Status == BorrowingStatus.Active && DateTime.UtcNow > DueDate;

    private BorrowingTransaction() { }

    public static BorrowingTransaction Create(
        Guid bookId,
        Guid memberId,
        Guid processedByUserId,
        int borrowDurationDays = 14,
        string? notes = null)
    {
        if (borrowDurationDays <= 0)
            throw new ArgumentException("Borrow duration must be positive.", nameof(borrowDurationDays));

        return new BorrowingTransaction
        {
            BookId = bookId,
            MemberId = memberId,
            ProcessedByUserId = processedByUserId,
            BorrowDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(borrowDurationDays),
            Status = BorrowingStatus.Active,
            Notes = notes?.Trim()
        };
    }

    public void Return(Guid returnProcessedByUserId, string? notes = null)
    {
        if (Status == BorrowingStatus.Returned)
            throw new InvalidOperationException("This book has already been returned.");
        if (Status == BorrowingStatus.Lost)
            throw new InvalidOperationException("Cannot return a book marked as lost.");

        ReturnDate = DateTime.UtcNow;
        ReturnProcessedByUserId = returnProcessedByUserId;
        Status = BorrowingStatus.Returned;

        if (notes is not null)
            Notes = notes.Trim();
    }

    public void MarkAsLost(Guid processedByUserId, string? notes = null)
    {
        if (Status == BorrowingStatus.Returned)
            throw new InvalidOperationException("Cannot mark a returned book as lost.");

        ReturnProcessedByUserId = processedByUserId;
        Status = BorrowingStatus.Lost;

        if (notes is not null)
            Notes = notes.Trim();
    }

    public void RefreshOverdueStatus()
    {
        if (Status == BorrowingStatus.Active && DateTime.UtcNow > DueDate)
            Status = BorrowingStatus.Overdue;
    }

    public void ExtendDueDate(int additionalDays)
    {
        if (Status != BorrowingStatus.Active && Status != BorrowingStatus.Overdue)
            throw new InvalidOperationException("Cannot extend a transaction that is not active or overdue.");
        if (additionalDays <= 0)
            throw new ArgumentException("Additional days must be positive.", nameof(additionalDays));

        DueDate = DueDate.AddDays(additionalDays);

        if (Status == BorrowingStatus.Overdue && DueDate > DateTime.UtcNow)
            Status = BorrowingStatus.Active;
    }
}
