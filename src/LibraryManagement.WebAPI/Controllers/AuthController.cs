using FluentValidation;
using LibraryManagement.Application.Common.Models;
using LibraryManagement.Application.DTOs.Auth;
using LibraryManagement.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.WebAPI.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IValidator<LoginRequest> _validator;

    public AuthController(IAuthService authService, IValidator<LoginRequest> validator)
    {
        _authService = authService;
        _validator = validator;
    }

    /// <summary>Authenticate and receive a JWT bearer token.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var validation = await _validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return BadRequest(ApiResponse<object>.Fail(
                "Validation failed", 400, validation.Errors.Select(e => e.ErrorMessage)));

        var result = await _authService.LoginAsync(request, ct);
        return Ok(ApiResponse<LoginResponse>.Ok(result, "Login successful"));
    }

    /// <summary>Returns the current user's identity claims (requires valid token).</summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Me()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value });
        return Ok(ApiResponse<object>.Ok(claims));
    }
}
