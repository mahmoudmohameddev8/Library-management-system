using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Interfaces.Repositories;
using LibraryManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Infrastructure.Repositories;

public class MemberRepository : GenericRepository<Member>, IMemberRepository
{
    public MemberRepository(LibraryDbContext context) : base(context) { }

    public async Task<Member?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await _context.Members
            .FirstOrDefaultAsync(m => m.Email == email.Trim().ToLowerInvariant(), cancellationToken);

    public async Task<Member?> GetByMembershipNumberAsync(string membershipNumber, CancellationToken cancellationToken = default)
        => await _context.Members
            .FirstOrDefaultAsync(m => m.MembershipNumber == membershipNumber.Trim(), cancellationToken);

    public async Task<Member?> GetWithTransactionsAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Members
            .Include(m => m.BorrowingTransactions)
                .ThenInclude(bt => bt.Book)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

    public async Task<bool> IsEmailUniqueAsync(
        string email,
        Guid? excludeMemberId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Members.Where(m => m.Email == email.Trim().ToLowerInvariant());

        if (excludeMemberId.HasValue)
            query = query.Where(m => m.Id != excludeMemberId.Value);

        return !await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> HasActiveBorrowingsAsync(Guid memberId, CancellationToken cancellationToken = default)
        => await _context.BorrowingTransactions
            .AnyAsync(bt => bt.MemberId == memberId &&
                            (bt.Status == BorrowingStatus.Active || bt.Status == BorrowingStatus.Overdue),
                      cancellationToken);

    public async Task<int> GetActiveBorrowingsCountAsync(Guid memberId, CancellationToken cancellationToken = default)
        => await _context.BorrowingTransactions
            .CountAsync(bt => bt.MemberId == memberId &&
                              (bt.Status == BorrowingStatus.Active || bt.Status == BorrowingStatus.Overdue),
                        cancellationToken);

    public async Task<string> GenerateMembershipNumberAsync(CancellationToken cancellationToken = default)
    {
        var count = await _context.Members.CountAsync(cancellationToken);
        return $"LIB-{(count + 1):D6}";
    }
}
