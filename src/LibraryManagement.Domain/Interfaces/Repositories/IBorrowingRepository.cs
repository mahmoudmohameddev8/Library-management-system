using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Domain.Interfaces.Repositories;

public interface IBorrowingRepository : IGenericRepository<BorrowingTransaction>
{
    Task<BorrowingTransaction?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<BorrowingTransaction>> GetByMemberAsync(Guid memberId, CancellationToken cancellationToken = default);
    Task<IEnumerable<BorrowingTransaction>> GetByBookAsync(Guid bookId, CancellationToken cancellationToken = default);
    Task<IEnumerable<BorrowingTransaction>> GetByStatusAsync(BorrowingStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<BorrowingTransaction>> GetOverdueTransactionsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<BorrowingTransaction>> GetActiveByMemberAsync(Guid memberId, CancellationToken cancellationToken = default);
    Task<BorrowingTransaction?> GetActiveForBookAsync(Guid bookId, Guid memberId, CancellationToken cancellationToken = default);
}
