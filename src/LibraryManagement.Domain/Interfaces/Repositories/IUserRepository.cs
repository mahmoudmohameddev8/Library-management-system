using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Domain.Interfaces.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetWithRoleAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetByRoleAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<bool> IsUsernameUniqueAsync(string username, Guid? excludeUserId = null, CancellationToken cancellationToken = default);
    Task<bool> IsEmailUniqueAsync(string email, Guid? excludeUserId = null, CancellationToken cancellationToken = default);
}
