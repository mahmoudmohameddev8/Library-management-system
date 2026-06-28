namespace LibraryManagement.Domain.Entities;

public class BookAuthor
{
    public Guid BookId { get; private set; }
    public Guid AuthorId { get; private set; }
    public bool IsPrimaryAuthor { get; private set; }

    public Book Book { get; private set; } = null!;
    public Author Author { get; private set; } = null!;

    private BookAuthor() { }

    public static BookAuthor Create(Guid bookId, Guid authorId, bool isPrimaryAuthor = false)
    {
        return new BookAuthor
        {
            BookId = bookId,
            AuthorId = authorId,
            IsPrimaryAuthor = isPrimaryAuthor
        };
    }
}
