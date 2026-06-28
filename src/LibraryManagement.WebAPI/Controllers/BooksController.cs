using FluentValidation;
using LibraryManagement.Application.Common.Models;
using LibraryManagement.Application.DTOs.Books;
using LibraryManagement.Application.Services;
using LibraryManagement.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.WebAPI.Controllers;

[ApiController]
[Route("api/books")]
[Authorize]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;
    private readonly IValidator<CreateBookRequest> _createValidator;
    private readonly IValidator<UpdateBookRequest> _updateValidator;

    public BooksController(
        IBookService bookService,
        IValidator<CreateBookRequest> createValidator,
        IValidator<UpdateBookRequest> updateValidator)
    {
        _bookService = bookService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    /// <summary>
    /// Get all books. Supports search by title/author/category, filter by status or category.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<BookDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] Guid? categoryId,
        CancellationToken ct)
    {
        BookStatus? bookStatus = null;
        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<BookStatus>(status, ignoreCase: true, out var parsed))
            bookStatus = parsed;

        var books = await _bookService.GetAllAsync(search, bookStatus, categoryId, ct);
        return Ok(ApiResponse<IEnumerable<BookDto>>.Ok(books));
    }

    /// <summary>Get a book with full details including category hierarchy and all authors.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<BookDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var book = await _bookService.GetByIdAsync(id, ct);
        return Ok(ApiResponse<BookDetailDto>.Ok(book));
    }

    /// <summary>Create a new book. Admin and Librarian only.</summary>
    [HttpPost]
    [Authorize(Policy = "AdminOrLibrarian")]
    [ProducesResponseType(typeof(ApiResponse<BookDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateBookRequest request, CancellationToken ct)
    {
        var validation = await _createValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return BadRequest(ApiResponse<object>.Fail(
                "Validation failed", 400, validation.Errors.Select(e => e.ErrorMessage)));

        var book = await _bookService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = book.Id },
            ApiResponse<BookDto>.Created(book));
    }

    /// <summary>Update an existing book. Admin and Librarian only.</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "AdminOrLibrarian")]
    [ProducesResponseType(typeof(ApiResponse<BookDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBookRequest request, CancellationToken ct)
    {
        var validation = await _updateValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return BadRequest(ApiResponse<object>.Fail(
                "Validation failed", 400, validation.Errors.Select(e => e.ErrorMessage)));

        var book = await _bookService.UpdateAsync(id, request, ct);
        return Ok(ApiResponse<BookDto>.Ok(book, "Book updated successfully"));
    }

    /// <summary>Delete a book. Admin only. Fails if the book has active borrowings.</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _bookService.DeleteAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(null, "Book deleted successfully"));
    }
}
