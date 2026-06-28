using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Domain.Interfaces.Repositories;

public interface IMemberRepository : IGenericRepository<Member>
{
    Task<Member?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Member?> GetByMembershipNumberAsync(string membershipNumber, CancellationToken cancellationToken = default);
    Task<Member?> GetWithTransactionsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> IsEmailUniqueAsync(string email, Guid? excludeMemberId = null, CancellationToken cancellationToken = default);
    Task<bool> HasActiveBorrowingsAsync(Guid memberId, CancellationToken cancellationToken = default);
    Task<int> GetActiveBorrowingsCountAsync(Guid memberId, CancellationToken cancellationToken = default);
    Task<string> GenerateMembershipNumberAsync(CancellationToken cancellationToken = default);
}
