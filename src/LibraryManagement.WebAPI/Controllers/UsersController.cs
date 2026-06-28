using FluentValidation;
using LibraryManagement.Application.Common.Models;
using LibraryManagement.Application.DTOs.Users;
using LibraryManagement.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.WebAPI.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Policy = "AdminOnly")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IValidator<CreateUserRequest> _createValidator;
    private readonly IValidator<UpdateUserRequest> _updateValidator;
    private readonly IValidator<ChangePasswordRequest> _passwordValidator;

    public UsersController(
        IUserService userService,
        IValidator<CreateUserRequest> createValidator,
        IValidator<UpdateUserRequest> updateValidator,
        IValidator<ChangePasswordRequest> passwordValidator)
    {
        _userService = userService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _passwordValidator = passwordValidator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var users = await _userService.GetAllAsync(ct);
        return Ok(ApiResponse<IEnumerable<UserDto>>.Ok(users));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var user = await _userService.GetByIdAsync(id, ct);
        return Ok(ApiResponse<UserDto>.Ok(user));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request, CancellationToken ct)
    {
        var validation = await _createValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return BadRequest(ApiResponse<object>.Fail(
                "Validation failed", 400, validation.Errors.Select(e => e.ErrorMessage)));

        var user = await _userService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = user.Id },
            ApiResponse<UserDto>.Created(user));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request, CancellationToken ct)
    {
        var validation = await _updateValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return BadRequest(ApiResponse<object>.Fail(
                "Validation failed", 400, validation.Errors.Select(e => e.ErrorMessage)));

        var user = await _userService.UpdateAsync(id, request, ct);
        return Ok(ApiResponse<UserDto>.Ok(user, "User updated successfully"));
    }

    [HttpPatch("{id:guid}/deactivate")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct)
    {
        await _userService.DeactivateAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(null, "User deactivated successfully"));
    }

    /// <summary>Change password. Any authenticated user can change their own; Admin can change any.</summary>
    [HttpPatch("{id:guid}/password")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePassword(
        Guid id,
        [FromBody] ChangePasswordRequest request,
        CancellationToken ct)
    {
        var validation = await _passwordValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return BadRequest(ApiResponse<object>.Fail(
                "Validation failed", 400, validation.Errors.Select(e => e.ErrorMessage)));

        await _userService.ChangePasswordAsync(id, request, ct);
        return Ok(ApiResponse<object>.Ok(null, "Password changed successfully"));
    }
}
