using LibraryManagement.Application.Common.Exceptions;
using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Application.DTOs.Books;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Interfaces.Repositories;

namespace LibraryManagement.Application.Services;

public class BookService : IBookService
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public BookService(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<IEnumerable<BookDto>> GetAllAsync(
        string? search = null,
        BookStatus? status = null,
        Guid? categoryId = null,
        CancellationToken ct = default)
    {
        IEnumerable<Book> books;

        if (!string.IsNullOrWhiteSpace(search))
            books = await _uow.Books.SearchAsync(search, ct);
        else if (status.HasValue)
            books = await _uow.Books.GetByStatusAsync(status.Value, ct);
        else if (categoryId.HasValue)
            books = await _uow.Books.GetByCategoryAsync(categoryId.Value, includeSubCategories: true, ct);
        else
            books = await _uow.Books.GetAllWithDetailsAsync(ct);

        return books.Select(MapToDto);
    }

    public async Task<BookDetailDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var book = await _uow.Books.GetWithDetailsAsync(id, ct)
            ?? throw new NotFoundException(nameof(Book), id);

        return MapToDetailDto(book);
    }

    public async Task<BookDto> CreateAsync(CreateBookRequest request, CancellationToken ct = default)
    {
        if (!await _uow.Books.IsIsbnUniqueAsync(request.ISBN, cancellationToken: ct))
            throw new ConflictException($"A book with ISBN '{request.ISBN}' already exists.");

        var book = Book.Create(
            request.ISBN, request.Title, request.Description,
            request.PublishedDate, request.TotalCopies,
            request.PublisherId, request.LanguageId, request.CategoryId,
            request.CoverImageUrl, request.PageCount);

        book.CreatedAt = DateTime.UtcNow;
        book.CreatedBy = _currentUser.Username;

        await _uow.Books.AddAsync(book, ct);
        await _uow.SaveChangesAsync(ct);

        foreach (var authorReq in request.Authors)
            book.AddAuthor(authorReq.AuthorId, authorReq.IsPrimaryAuthor);

        await _uow.SaveChangesAsync(ct);

        var created = await _uow.Books.GetWithDetailsAsync(book.Id, ct) ?? book;
        return MapToDto(created);
    }

    public async Task<BookDto> UpdateAsync(Guid id, UpdateBookRequest request, CancellationToken ct = default)
    {
        var book = await _uow.Books.GetWithDetailsAsync(id, ct)
            ?? throw new NotFoundException(nameof(Book), id);

        book.Update(
            request.Title, request.Description, request.PublishedDate,
            request.TotalCopies, request.PublisherId, request.LanguageId,
            request.CategoryId, request.CoverImageUrl, request.PageCount);

        book.UpdatedAt = DateTime.UtcNow;
        book.UpdatedBy = _currentUser.Username;

        await _uow.Books.UpdateAsync(book, ct);
        await _uow.SaveChangesAsync(ct);

        return MapToDto(book);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var book = await _uow.Books.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Book), id);

        var hasActiveBorrowings = await _uow.BorrowingTransactions
            .FindAsync(bt => bt.BookId == id &&
                            (bt.Status == Domain.Enums.BorrowingStatus.Active ||
                             bt.Status == Domain.Enums.BorrowingStatus.Overdue), ct);

        if (hasActiveBorrowings.Any())
            throw new ConflictException("Cannot delete a book that has active borrowings.");

        await _uow.Books.DeleteAsync(book, ct);
        await _uow.SaveChangesAsync(ct);
    }

    private static BookDto MapToDto(Book book) => new(
        book.Id,
        book.ISBN,
        book.Title,
        book.Description,
        book.PublishedDate,
        book.TotalCopies,
        book.AvailableCopies,
        book.Status.ToString(),
        book.Category?.Name ?? string.Empty,
        book.Publisher?.Name ?? string.Empty,
        book.Language?.Name ?? string.Empty,
        book.BookAuthors.Select(ba => ba.Author?.FullName ?? string.Empty)
    );

    private static BookDetailDto MapToDetailDto(Book book) => new(
        book.Id,
        book.ISBN,
        book.Title,
        book.Description,
        book.PublishedDate,
        book.TotalCopies,
        book.AvailableCopies,
        book.Status.ToString(),
        book.CoverImageUrl,
        book.PageCount,
        book.CategoryId,
        book.Category?.Name ?? string.Empty,
        book.Category?.Parent?.Name,
        book.PublisherId,
        book.Publisher?.Name ?? string.Empty,
        book.LanguageId,
        book.Language?.Name ?? string.Empty,
        book.BookAuthors.Select(ba => new AuthorSummaryDto(
            ba.AuthorId,
            ba.Author?.FullName ?? string.Empty,
            ba.IsPrimaryAuthor)),
        book.CreatedAt,
        book.CreatedBy
    );
}
