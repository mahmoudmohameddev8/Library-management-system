using FluentValidation;
using LibraryManagement.Application.Common.Models;
using LibraryManagement.Application.DTOs.Borrowing;
using LibraryManagement.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.WebAPI.Controllers;

[ApiController]
[Route("api/borrowing")]
[Authorize(Policy = "AllStaff")]
public class BorrowingController : ControllerBase
{
    private readonly IBorrowingService _borrowingService;
    private readonly IValidator<BorrowBookRequest> _borrowValidator;

    public BorrowingController(
        IBorrowingService borrowingService,
        IValidator<BorrowBookRequest> borrowValidator)
    {
        _borrowingService = borrowingService;
        _borrowValidator = borrowValidator;
    }

    /// <summary>
    /// Borrow a book for a member. Uses optimistic concurrency to prevent
    /// two staff members from lending the last copy simultaneously.
    /// </summary>
    [HttpPost("borrow")]
    [ProducesResponseType(typeof(ApiResponse<BorrowingTransactionDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Borrow([FromBody] BorrowBookRequest request, CancellationToken ct)
    {
        var validation = await _borrowValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return BadRequest(ApiResponse<object>.Fail(
                "Validation failed", 400, validation.Errors.Select(e => e.ErrorMessage)));

        var transaction = await _borrowingService.BorrowAsync(request, ct);
        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<BorrowingTransactionDto>.Created(transaction, "Book borrowed successfully"));
    }

    /// <summary>Return a borrowed book.</summary>
    [HttpPost("{transactionId:guid}/return")]
    [ProducesResponseType(typeof(ApiResponse<BorrowingTransactionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Return(
        Guid transactionId,
        [FromBody] ReturnBookRequest request,
        CancellationToken ct)
    {
        var transaction = await _borrowingService.ReturnAsync(transactionId, request, ct);
        return Ok(ApiResponse<BorrowingTransactionDto>.Ok(transaction, "Book returned successfully"));
    }

    /// <summary>Get borrowing history for a specific member.</summary>
    [HttpGet("member/{memberId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<BorrowingTransactionDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByMember(Guid memberId, CancellationToken ct)
    {
        var transactions = await _borrowingService.GetByMemberAsync(memberId, ct);
        return Ok(ApiResponse<IEnumerable<BorrowingTransactionDto>>.Ok(transactions));
    }

    /// <summary>Get all currently active borrowings.</summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<BorrowingTransactionDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActive(CancellationToken ct)
    {
        var transactions = await _borrowingService.GetAllActiveAsync(ct);
        return Ok(ApiResponse<IEnumerable<BorrowingTransactionDto>>.Ok(transactions));
    }

    /// <summary>Get all overdue borrowings (past due date, not yet returned).</summary>
    [HttpGet("overdue")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<BorrowingTransactionDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOverdue(CancellationToken ct)
    {
        var transactions = await _borrowingService.GetOverdueAsync(ct);
        return Ok(ApiResponse<IEnumerable<BorrowingTransactionDto>>.Ok(transactions));
    }
}
