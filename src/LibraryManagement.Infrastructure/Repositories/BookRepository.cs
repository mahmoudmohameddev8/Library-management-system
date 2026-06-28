using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Interfaces.Repositories;
using LibraryManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Infrastructure.Repositories;

public class BookRepository : GenericRepository<Book>, IBookRepository
{
    public BookRepository(LibraryDbContext context) : base(context) { }

    public async Task<Book?> GetByIsbnAsync(string isbn, CancellationToken cancellationToken = default)
        => await _context.Books
            .FirstOrDefaultAsync(b => b.ISBN == isbn.Trim(), cancellationToken);

    public async Task<Book?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Books
            .Include(b => b.BookAuthors)
                .ThenInclude(ba => ba.Author)
            .Include(b => b.Category)
                .ThenInclude(c => c.Parent)
            .Include(b => b.Publisher)
            .Include(b => b.Language)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

    public async Task<IEnumerable<Book>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default)
        => await _context.Books
            .Include(b => b.BookAuthors)
                .ThenInclude(ba => ba.Author)
            .Include(b => b.Category)
            .Include(b => b.Publisher)
            .Include(b => b.Language)
            .OrderBy(b => b.Title)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Book>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var term = searchTerm.Trim().ToLower();

        return await _context.Books
            .Include(b => b.BookAuthors)
                .ThenInclude(ba => ba.Author)
            .Include(b => b.Category)
            .Include(b => b.Publisher)
            .Include(b => b.Language)
            .Where(b =>
                b.Title.ToLower().Contains(term) ||
                b.ISBN.Contains(term) ||
                b.BookAuthors.Any(ba =>
                    ba.Author.FirstName.ToLower().Contains(term) ||
                    ba.Author.LastName.ToLower().Contains(term)) ||
                b.Category.Name.ToLower().Contains(term))
            .OrderBy(b => b.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Book>> GetByStatusAsync(BookStatus status, CancellationToken cancellationToken = default)
        => await _context.Books
            .Include(b => b.BookAuthors)
                .ThenInclude(ba => ba.Author)
            .Include(b => b.Category)
            .Where(b => b.Status == status)
            .OrderBy(b => b.Title)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Book>> GetByCategoryAsync(
        Guid categoryId,
        bool includeSubCategories = false,
        CancellationToken cancellationToken = default)
    {
        if (!includeSubCategories)
        {
            return await _context.Books
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .Include(b => b.Category)
                .Where(b => b.CategoryId == categoryId)
                .OrderBy(b => b.Title)
                .ToListAsync(cancellationToken);
        }

        // Collect all descendant category IDs for hierarchical search
        var categoryIds = await GetCategoryAndDescendantIdsAsync(categoryId, cancellationToken);

        return await _context.Books
            .Include(b => b.BookAuthors)
                .ThenInclude(ba => ba.Author)
            .Include(b => b.Category)
            .Where(b => categoryIds.Contains(b.CategoryId))
            .OrderBy(b => b.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Book>> GetByAuthorAsync(Guid authorId, CancellationToken cancellationToken = default)
        => await _context.Books
            .Include(b => b.BookAuthors)
                .ThenInclude(ba => ba.Author)
            .Include(b => b.Category)
            .Include(b => b.Publisher)
            .Where(b => b.BookAuthors.Any(ba => ba.AuthorId == authorId))
            .OrderBy(b => b.Title)
            .ToListAsync(cancellationToken);

    public async Task<bool> IsIsbnUniqueAsync(
        string isbn,
        Guid? excludeBookId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Books.Where(b => b.ISBN == isbn.Trim());

        if (excludeBookId.HasValue)
            query = query.Where(b => b.Id != excludeBookId.Value);

        return !await query.AnyAsync(cancellationToken);
    }

    private async Task<List<Guid>> GetCategoryAndDescendantIdsAsync(
        Guid categoryId,
        CancellationToken cancellationToken)
    {
        var result = new List<Guid> { categoryId };
        var queue = new Queue<Guid>();
        queue.Enqueue(categoryId);

        while (queue.Count > 0)
        {
            var currentId = queue.Dequeue();
            var childIds = await _context.Categories
                .Where(c => c.ParentCategoryId == currentId)
                .Select(c => c.Id)
                .ToListAsync(cancellationToken);

            foreach (var childId in childIds)
            {
                result.Add(childId);
                queue.Enqueue(childId);
            }
        }

        return result;
    }
}
