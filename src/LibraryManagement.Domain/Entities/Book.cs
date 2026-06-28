using LibraryManagement.Domain.Common;
using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Domain.Entities;

public class Book : AuditableEntity
{
    private readonly List<BookAuthor> _bookAuthors = new();
    private readonly List<BorrowingTransaction> _borrowingTransactions = new();

    public string ISBN { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public DateOnly? PublishedDate { get; private set; }
    public int TotalCopies { get; private set; }
    public int AvailableCopies { get; private set; }
    public BookStatus Status { get; private set; }
    public string? CoverImageUrl { get; private set; }
    public int? PageCount { get; private set; }

    public Guid PublisherId { get; private set; }
    public Guid LanguageId { get; private set; }
    public Guid CategoryId { get; private set; }

    public Publisher Publisher { get; private set; } = null!;
    public Language Language { get; private set; } = null!;
    public Category Category { get; private set; } = null!;

    public IReadOnlyCollection<BookAuthor> BookAuthors => _bookAuthors.AsReadOnly();
    public IReadOnlyCollection<BorrowingTransaction> BorrowingTransactions => _borrowingTransactions.AsReadOnly();

    private Book() { }

    public static Book Create(
        string isbn,
        string title,
        string? description,
        DateOnly? publishedDate,
        int totalCopies,
        Guid publisherId,
        Guid languageId,
        Guid categoryId,
        string? coverImageUrl = null,
        int? pageCount = null,
        Guid? id = null)
    {
        if (string.IsNullOrWhiteSpace(isbn))
            throw new ArgumentException("ISBN cannot be empty.", nameof(isbn));
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.", nameof(title));
        if (totalCopies < 0)
            throw new ArgumentException("Total copies cannot be negative.", nameof(totalCopies));

        return new Book
        {
            Id = id ?? Guid.NewGuid(),
            ISBN = isbn.Trim(),
            Title = title.Trim(),
            Description = description?.Trim(),
            PublishedDate = publishedDate,
            TotalCopies = totalCopies,
            AvailableCopies = totalCopies,
            Status = totalCopies > 0 ? BookStatus.Available : BookStatus.FullyBorrowed,
            PublisherId = publisherId,
            LanguageId = languageId,
            CategoryId = categoryId,
            CoverImageUrl = coverImageUrl?.Trim(),
            PageCount = pageCount
        };
    }

    public void Update(
        string title,
        string? description,
        DateOnly? publishedDate,
        int totalCopies,
        Guid publisherId,
        Guid languageId,
        Guid categoryId,
        string? coverImageUrl = null,
        int? pageCount = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.", nameof(title));

        int borrowedCopies = TotalCopies - AvailableCopies;
        if (totalCopies < borrowedCopies)
            throw new InvalidOperationException(
                $"Cannot reduce total copies below the number of currently borrowed copies ({borrowedCopies}).");

        Title = title.Trim();
        Description = description?.Trim();
        PublishedDate = publishedDate;
        TotalCopies = totalCopies;
        AvailableCopies = totalCopies - borrowedCopies;
        PublisherId = publisherId;
        LanguageId = languageId;
        CategoryId = categoryId;
        CoverImageUrl = coverImageUrl?.Trim();
        PageCount = pageCount;

        RecalculateStatus();
    }

    public bool TryBorrow()
    {
        if (AvailableCopies <= 0 || Status == BookStatus.Maintenance || Status == BookStatus.Discontinued)
            return false;

        AvailableCopies--;
        RecalculateStatus();
        return true;
    }

    public void Return()
    {
        if (AvailableCopies >= TotalCopies)
            throw new InvalidOperationException("Cannot return a copy when none are currently borrowed.");

        AvailableCopies++;
        RecalculateStatus();
    }

    public void SetStatus(BookStatus status)
    {
        Status = status;
    }

    public void AddAuthor(Guid authorId, bool isPrimaryAuthor = false)
    {
        if (_bookAuthors.Any(ba => ba.AuthorId == authorId))
            return;
        _bookAuthors.Add(BookAuthor.Create(Id, authorId, isPrimaryAuthor));
    }

    public void ReplaceAuthors(IEnumerable<(Guid AuthorId, bool IsPrimaryAuthor)> authors)
    {
        _bookAuthors.Clear();
        foreach (var (authorId, isPrimary) in authors)
            _bookAuthors.Add(BookAuthor.Create(Id, authorId, isPrimary));
    }

    private void RecalculateStatus()
    {
        if (Status == BookStatus.Maintenance || Status == BookStatus.Discontinued)
            return;

        Status = AvailableCopies > 0 ? BookStatus.Available : BookStatus.FullyBorrowed;
    }
}
