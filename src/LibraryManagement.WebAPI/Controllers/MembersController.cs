using FluentValidation;
using LibraryManagement.Application.Common.Models;
using LibraryManagement.Application.DTOs.Members;
using LibraryManagement.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.WebAPI.Controllers;

[ApiController]
[Route("api/members")]
[Authorize(Policy = "AdminOrLibrarian")]
public class MembersController : ControllerBase
{
    private readonly IMemberService _memberService;
    private readonly IValidator<CreateMemberRequest> _createValidator;
    private readonly IValidator<UpdateMemberRequest> _updateValidator;

    public MembersController(
        IMemberService memberService,
        IValidator<CreateMemberRequest> createValidator,
        IValidator<UpdateMemberRequest> updateValidator)
    {
        _memberService = memberService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<MemberDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var members = await _memberService.GetAllAsync(ct);
        return Ok(ApiResponse<IEnumerable<MemberDto>>.Ok(members));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<MemberDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var member = await _memberService.GetByIdAsync(id, ct);
        return Ok(ApiResponse<MemberDto>.Ok(member));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<MemberDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateMemberRequest request, CancellationToken ct)
    {
        var validation = await _createValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return BadRequest(ApiResponse<object>.Fail(
                "Validation failed", 400, validation.Errors.Select(e => e.ErrorMessage)));

        var member = await _memberService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = member.Id },
            ApiResponse<MemberDto>.Created(member));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<MemberDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMemberRequest request, CancellationToken ct)
    {
        var validation = await _updateValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return BadRequest(ApiResponse<object>.Fail(
                "Validation failed", 400, validation.Errors.Select(e => e.ErrorMessage)));

        var member = await _memberService.UpdateAsync(id, request, ct);
        return Ok(ApiResponse<MemberDto>.Ok(member, "Member updated successfully"));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _memberService.DeleteAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(null, "Member deleted successfully"));
    }

    [HttpPatch("{id:guid}/suspend")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Suspend(Guid id, CancellationToken ct)
    {
        await _memberService.SuspendAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(null, "Member suspended successfully"));
    }

    [HttpPatch("{id:guid}/renew")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RenewMembership(
        Guid id,
        [FromBody] RenewMembershipRequest request,
        CancellationToken ct)
    {
        await _memberService.RenewMembershipAsync(id, request.Months, ct);
        return Ok(ApiResponse<object>.Ok(null, "Membership renewed successfully"));
    }
}
