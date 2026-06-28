namespace LibraryManagement.Domain.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    IBookRepository Books { get; }
    IMemberRepository Members { get; }
    IUserRepository Users { get; }
    IBorrowingRepository BorrowingTransactions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    /// <summary>Returns false instead of throwing when a concurrency conflict is detected.</summary>
    Task<bool> TrySaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
