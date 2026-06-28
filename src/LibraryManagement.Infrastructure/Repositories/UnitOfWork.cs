using LibraryManagement.Domain.Interfaces.Repositories;
using LibraryManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace LibraryManagement.Infrastructure.Repositories;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly LibraryDbContext _context;
    private IDbContextTransaction? _currentTransaction;

    public IBookRepository Books { get; }
    public IMemberRepository Members { get; }
    public IUserRepository Users { get; }
    public IBorrowingRepository BorrowingTransactions { get; }

    public UnitOfWork(
        LibraryDbContext context,
        IBookRepository books,
        IMemberRepository members,
        IUserRepository users,
        IBorrowingRepository borrowingTransactions)
    {
        _context = context;
        Books = books;
        Members = members;
        Users = users;
        BorrowingTransactions = borrowingTransactions;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    public async Task<bool> TrySaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
        {
            return false;
        }
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is not null)
            throw new InvalidOperationException("A transaction is already in progress.");

        _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is null)
            throw new InvalidOperationException("No active transaction to commit.");

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            await _currentTransaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is null)
            return;

        await _currentTransaction.RollbackAsync(cancellationToken);
        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
    }

    public void Dispose()
    {
        _currentTransaction?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_currentTransaction is not null)
            await _currentTransaction.DisposeAsync();
    }
}
