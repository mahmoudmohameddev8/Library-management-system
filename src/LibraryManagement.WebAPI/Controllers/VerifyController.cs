using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.WebAPI.Controllers;

/// <summary>
/// Temporary read-only controller to verify Step 1 seeding and DB setup.
/// Will be replaced by full CRUD controllers in Step 3.
/// </summary>
[ApiController]
[Route("api/verify")]
public class VerifyController : ControllerBase
{
    private readonly IUnitOfWork _uow;

    public VerifyController(IUnitOfWork uow) => _uow = uow;

    [HttpGet("roles")]
    public async Task<IActionResult> GetRoles()
    {
        var roles = await _uow.Users.GetAllAsync();
        var result = await Task.FromResult(new[]
        {
            new { id = "00000000-0000-0000-0000-000000000001", name = "Admin" },
            new { id = "00000000-0000-0000-0000-000000000002", name = "Librarian" },
            new { id = "00000000-0000-0000-0000-000000000003", name = "Staff" }
        });
        return Ok(result);
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _uow.Users.GetAllAsync();
        var result = users.Select(u => new
        {
            u.Id,
            u.Username,
            u.Email,
            u.FirstName,
            u.LastName,
            u.IsActive,
            RoleId = u.RoleId
        });
        return Ok(result);
    }

    [HttpGet("books")]
    public async Task<IActionResult> GetBooks()
    {
        var books = await _uow.Books.GetAllWithDetailsAsync();
        var result = books.Select(b => new
        {
            b.Id,
            b.ISBN,
            b.Title,
            b.TotalCopies,
            b.AvailableCopies,
            Status = b.Status.ToString(),
            Category = b.Category?.Name,
            Publisher = b.Publisher?.Name,
            Language = b.Language?.Name,
            Authors = b.BookAuthors.Select(ba => new
            {
                ba.Author.FullName,
                ba.IsPrimaryAuthor
            })
        });
        return Ok(result);
    }

    [HttpGet("members")]
    public async Task<IActionResult> GetMembers()
    {
        var members = await _uow.Members.GetAllAsync();
        var result = members.Select(m => new
        {
            m.Id,
            m.MembershipNumber,
            m.FullName,
            m.Email,
            Status = m.Status.ToString(),
            m.MembershipExpiryDate
        });
        return Ok(result);
    }

    [HttpGet("books/available")]
    public async Task<IActionResult> GetAvailableBooks()
    {
        var books = await _uow.Books.GetByStatusAsync(BookStatus.Available);
        return Ok(books.Select(b => new { b.Id, b.Title, b.AvailableCopies }));
    }

    [HttpGet("health")]
    public IActionResult Health() =>
        Ok(new { status = "Healthy", timestamp = DateTime.UtcNow, step = "Step 1 — Domain + Infrastructure" });
}
