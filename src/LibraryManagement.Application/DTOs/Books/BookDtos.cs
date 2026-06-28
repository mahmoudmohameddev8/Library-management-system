namespace LibraryManagement.Application.DTOs.Books;

public record BookDto(
    Guid Id,
    string ISBN,
    string Title,
    string? Description,
    DateOnly? PublishedDate,
    int TotalCopies,
    int AvailableCopies,
    string Status,
    string CategoryName,
    string PublisherName,
    string LanguageName,
    IEnumerable<string> Authors
);

public record BookDetailDto(
    Guid Id,
    string ISBN,
    string Title,
    string? Description,
    DateOnly? PublishedDate,
    int TotalCopies,
    int AvailableCopies,
    string Status,
    string? CoverImageUrl,
    int? PageCount,
    Guid CategoryId,
    string CategoryName,
    string? ParentCategoryName,
    Guid PublisherId,
    string PublisherName,
    Guid LanguageId,
    string LanguageName,
    IEnumerable<AuthorSummaryDto> Authors,
    DateTime CreatedAt,
    string CreatedBy
);

public record AuthorSummaryDto(Guid Id, string FullName, bool IsPrimaryAuthor);

public record CreateBookRequest(
    string ISBN,
    string Title,
    string? Description,
    DateOnly? PublishedDate,
    int TotalCopies,
    Guid PublisherId,
    Guid LanguageId,
    Guid CategoryId,
    IEnumerable<BookAuthorRequest> Authors,
    string? CoverImageUrl = null,
    int? PageCount = null
);

public record BookAuthorRequest(Guid AuthorId, bool IsPrimaryAuthor = false);

public record UpdateBookRequest(
    string Title,
    string? Description,
    DateOnly? PublishedDate,
    int TotalCopies,
    Guid PublisherId,
    Guid LanguageId,
    Guid CategoryId,
    string? CoverImageUrl = null,
    int? PageCount = null
);
