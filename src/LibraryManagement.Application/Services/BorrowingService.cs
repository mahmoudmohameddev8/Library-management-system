using LibraryManagement.Application.Common.Exceptions;
using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Application.DTOs.Borrowing;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Interfaces.Repositories;

namespace LibraryManagement.Application.Services;

public class BorrowingService : IBorrowingService
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public BorrowingService(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<BorrowingTransactionDto> BorrowAsync(BorrowBookRequest request, CancellationToken ct = default)
    {
        var member = await _uow.Members.GetByIdAsync(request.MemberId, ct)
            ?? throw new NotFoundException(nameof(Member), request.MemberId);

        var activeBorrows = await _uow.Members.GetActiveBorrowingsCountAsync(request.MemberId, ct);

        if (!member.CanBorrow(activeBorrows))
            throw new ConflictException(
                member.Status != MembershipStatus.Active
                    ? $"Member '{member.FullName}' has a suspended or expired membership."
                    : $"Member '{member.FullName}' has reached the maximum borrow limit of {member.MaxBorrowLimit} books.");

        var book = await _uow.Books.GetWithDetailsAsync(request.BookId, ct)
            ?? throw new NotFoundException(nameof(Book), request.BookId);

        var existingActive = await _uow.BorrowingTransactions
            .GetActiveForBookAsync(request.BookId, request.MemberId, ct);

        if (existingActive is not null)
            throw new ConflictException($"Member '{member.FullName}' has already borrowed this book.");

        await _uow.BeginTransactionAsync(ct);
        try
        {
            if (!book.TryBorrow())
                throw new ConflictException($"'{book.Title}' has no available copies.");

            book.UpdatedAt = DateTime.UtcNow;
            book.UpdatedBy = _currentUser.Username;
            await _uow.Books.UpdateAsync(book, ct);

            var transaction = BorrowingTransaction.Create(
                request.BookId, request.MemberId, _currentUser.UserId,
                request.BorrowDurationDays, request.Notes);

            transaction.CreatedAt = DateTime.UtcNow;
            transaction.CreatedBy = _currentUser.Username;

            await _uow.BorrowingTransactions.AddAsync(transaction, ct);

            // TrySaveChangesAsync catches DbUpdateConcurrencyException from the RowVersion check
            // so the Application layer doesn't need to reference EF Core directly.
            if (!await _uow.TrySaveChangesAsync(ct))
            {
                await _uow.RollbackTransactionAsync(ct);
                throw new ConflictException(
                    "This copy was just taken by another user. Please try again.");
            }

            await _uow.CommitTransactionAsync(ct);

            return MapToDto(transaction, book, member, _currentUser.Username);
        }
        catch
        {
            await _uow.RollbackTransactionAsync(ct);
            throw;
        }
    }

    public async Task<BorrowingTransactionDto> ReturnAsync(
        Guid transactionId, ReturnBookRequest request, CancellationToken ct = default)
    {
        var transaction = await _uow.BorrowingTransactions.GetWithDetailsAsync(transactionId, ct)
            ?? throw new NotFoundException(nameof(BorrowingTransaction), transactionId);

        if (transaction.Status == BorrowingStatus.Returned)
            throw new ConflictException("This book has already been returned.");

        await _uow.BeginTransactionAsync(ct);
        try
        {
            transaction.Return(_currentUser.UserId, request.Notes);
            transaction.UpdatedAt = DateTime.UtcNow;
            transaction.UpdatedBy = _currentUser.Username;

            transaction.Book.Return();
            transaction.Book.UpdatedAt = DateTime.UtcNow;
            transaction.Book.UpdatedBy = _currentUser.Username;

            await _uow.BorrowingTransactions.UpdateAsync(transaction, ct);
            await _uow.Books.UpdateAsync(transaction.Book, ct);
            await _uow.CommitTransactionAsync(ct);

            return MapToDto(transaction, transaction.Book, transaction.Member, _currentUser.Username);
        }
        catch
        {
            await _uow.RollbackTransactionAsync(ct);
            throw;
        }
    }

    public async Task<IEnumerable<BorrowingTransactionDto>> GetByMemberAsync(
        Guid memberId, CancellationToken ct = default)
    {
        _ = await _uow.Members.GetByIdAsync(memberId, ct)
            ?? throw new NotFoundException(nameof(Member), memberId);

        var transactions = await _uow.BorrowingTransactions.GetByMemberAsync(memberId, ct);
        return transactions.Select(t => MapToDto(t, t.Book, t.Member, t.ProcessedByUser?.Username ?? string.Empty));
    }

    public async Task<IEnumerable<BorrowingTransactionDto>> GetOverdueAsync(CancellationToken ct = default)
    {
        var transactions = await _uow.BorrowingTransactions.GetOverdueTransactionsAsync(ct);
        return transactions.Select(t => MapToDto(t, t.Book, t.Member, t.ProcessedByUser?.Username ?? string.Empty));
    }

    public async Task<IEnumerable<BorrowingTransactionDto>> GetAllActiveAsync(CancellationToken ct = default)
    {
        var transactions = await _uow.BorrowingTransactions.GetByStatusAsync(BorrowingStatus.Active, ct);
        return transactions.Select(t => MapToDto(t, t.Book, t.Member, t.ProcessedByUser?.Username ?? string.Empty));
    }

    private static BorrowingTransactionDto MapToDto(
        BorrowingTransaction t, Book book, Member member, string processedByUsername) => new(
        t.Id,
        book.Id,
        book.Title,
        book.ISBN,
        member.Id,
        member.FullName,
        member.MembershipNumber,
        t.ProcessedByUserId,
        processedByUsername,
        t.BorrowDate,
        t.DueDate,
        t.ReturnDate,
        t.Status.ToString(),
        t.IsOverdue,
        t.Notes);
}
