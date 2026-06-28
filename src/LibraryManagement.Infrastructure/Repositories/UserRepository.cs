using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Interfaces.Repositories;
using LibraryManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(LibraryDbContext context) : base(context) { }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
        => await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Username == username.Trim().ToLowerInvariant(), cancellationToken);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == email.Trim().ToLowerInvariant(), cancellationToken);

    public async Task<User?> GetWithRoleAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public async Task<IEnumerable<User>> GetByRoleAsync(Guid roleId, CancellationToken cancellationToken = default)
        => await _context.Users
            .Include(u => u.Role)
            .Where(u => u.RoleId == roleId)
            .OrderBy(u => u.LastName)
            .ToListAsync(cancellationToken);

    public async Task<bool> IsUsernameUniqueAsync(
        string username,
        Guid? excludeUserId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Users.Where(u => u.Username == username.Trim().ToLowerInvariant());

        if (excludeUserId.HasValue)
            query = query.Where(u => u.Id != excludeUserId.Value);

        return !await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> IsEmailUniqueAsync(
        string email,
        Guid? excludeUserId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Users.Where(u => u.Email == email.Trim().ToLowerInvariant());

        if (excludeUserId.HasValue)
            query = query.Where(u => u.Id != excludeUserId.Value);

        return !await query.AnyAsync(cancellationToken);
    }
}
