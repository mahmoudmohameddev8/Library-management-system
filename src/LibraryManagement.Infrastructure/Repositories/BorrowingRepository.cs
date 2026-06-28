using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Interfaces.Repositories;
using LibraryManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Infrastructure.Repositories;

public class BorrowingRepository : GenericRepository<BorrowingTransaction>, IBorrowingRepository
{
    public BorrowingRepository(LibraryDbContext context) : base(context) { }

    public async Task<BorrowingTransaction?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.BorrowingTransactions
            .Include(bt => bt.Book)
                .ThenInclude(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
            .Include(bt => bt.Member)
            .Include(bt => bt.ProcessedByUser)
                .ThenInclude(u => u.Role)
            .Include(bt => bt.ReturnProcessedByUser)
            .FirstOrDefaultAsync(bt => bt.Id == id, cancellationToken);

    public async Task<IEnumerable<BorrowingTransaction>> GetByMemberAsync(
        Guid memberId,
        CancellationToken cancellationToken = default)
        => await _context.BorrowingTransactions
            .Include(bt => bt.Book)
            .Include(bt => bt.ProcessedByUser)
            .Where(bt => bt.MemberId == memberId)
            .OrderByDescending(bt => bt.BorrowDate)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<BorrowingTransaction>> GetByBookAsync(
        Guid bookId,
        CancellationToken cancellationToken = default)
        => await _context.BorrowingTransactions
            .Include(bt => bt.Member)
            .Include(bt => bt.ProcessedByUser)
            .Where(bt => bt.BookId == bookId)
            .OrderByDescending(bt => bt.BorrowDate)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<BorrowingTransaction>> GetByStatusAsync(
        BorrowingStatus status,
        CancellationToken cancellationToken = default)
        => await _context.BorrowingTransactions
            .Include(bt => bt.Book)
            .Include(bt => bt.Member)
            .Where(bt => bt.Status == status)
            .OrderByDescending(bt => bt.BorrowDate)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<BorrowingTransaction>> GetOverdueTransactionsAsync(
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        return await _context.BorrowingTransactions
            .Include(bt => bt.Book)
            .Include(bt => bt.Member)
            .Where(bt => (bt.Status == BorrowingStatus.Active || bt.Status == BorrowingStatus.Overdue)
                         && bt.DueDate < now)
            .OrderBy(bt => bt.DueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<BorrowingTransaction>> GetActiveByMemberAsync(
        Guid memberId,
        CancellationToken cancellationToken = default)
        => await _context.BorrowingTransactions
            .Include(bt => bt.Book)
            .Where(bt => bt.MemberId == memberId &&
                         (bt.Status == BorrowingStatus.Active || bt.Status == BorrowingStatus.Overdue))
            .ToListAsync(cancellationToken);

    public async Task<BorrowingTransaction?> GetActiveForBookAsync(
        Guid bookId,
        Guid memberId,
        CancellationToken cancellationToken = default)
        => await _context.BorrowingTransactions
            .FirstOrDefaultAsync(bt => bt.BookId == bookId &&
                                       bt.MemberId == memberId &&
                                       (bt.Status == BorrowingStatus.Active || bt.Status == BorrowingStatus.Overdue),
                                 cancellationToken);
}
