using LibraryManagement.Domain.Common;

namespace LibraryManagement.Domain.Entities;

public class Category : AuditableEntity
{
    private readonly List<Category> _children = new();
    private readonly List<Book> _books = new();

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Guid? ParentCategoryId { get; private set; }

    public Category? Parent { get; private set; }
    public IReadOnlyCollection<Category> Children => _children.AsReadOnly();
    public IReadOnlyCollection<Book> Books => _books.AsReadOnly();

    public bool IsRoot => ParentCategoryId is null;

    private Category() { }

    public static Category Create(string name, string? description = null, Guid? parentCategoryId = null, Guid? id = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be empty.", nameof(name));

        return new Category
        {
            Id = id ?? Guid.NewGuid(),
            Name = name.Trim(),
            Description = description?.Trim(),
            ParentCategoryId = parentCategoryId
        };
    }

    public void Update(string name, string? description = null, Guid? parentCategoryId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be empty.", nameof(name));

        Name = name.Trim();
        Description = description?.Trim();
        ParentCategoryId = parentCategoryId;
    }
}
