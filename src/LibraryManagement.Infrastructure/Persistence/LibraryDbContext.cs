using System.Reflection;
using LibraryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Infrastructure.Persistence;

public class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }

    public DbSet<Book> Books => Set<Book>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<BookAuthor> BookAuthors => Set<BookAuthor>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Publisher> Publishers => Set<Publisher>();
    public DbSet<Language> Languages => Set<Language>();
    public DbSet<Member> Members => Set<Member>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<BorrowingTransaction> BorrowingTransactions => Set<BorrowingTransaction>();
    public DbSet<UserActivityLog> UserActivityLogs => Set<UserActivityLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            return;

        optionsBuilder.EnableSensitiveDataLogging(false);
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
    }
}
