using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Domain.Interfaces.Repositories;

public interface IBookRepository : IGenericRepository<Book>
{
    Task<Book?> GetByIsbnAsync(string isbn, CancellationToken cancellationToken = default);
    Task<Book?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Book>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Book>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<IEnumerable<Book>> GetByStatusAsync(BookStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Book>> GetByCategoryAsync(Guid categoryId, bool includeSubCategories = false, CancellationToken cancellationToken = default);
    Task<IEnumerable<Book>> GetByAuthorAsync(Guid authorId, CancellationToken cancellationToken = default);
    Task<bool> IsIsbnUniqueAsync(string isbn, Guid? excludeBookId = null, CancellationToken cancellationToken = default);
}
