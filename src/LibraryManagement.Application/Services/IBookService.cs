using LibraryManagement.Application.DTOs.Books;
using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Application.Services;

public interface IBookService
{
    Task<IEnumerable<BookDto>> GetAllAsync(
        string? search = null,
        BookStatus? status = null,
        Guid? categoryId = null,
        CancellationToken ct = default);

    Task<BookDetailDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<BookDto> CreateAsync(CreateBookRequest request, CancellationToken ct = default);
    Task<BookDto> UpdateAsync(Guid id, UpdateBookRequest request, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
