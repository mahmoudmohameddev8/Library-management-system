using LibraryManagement.Application.Common.Exceptions;
using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Application.DTOs.Auth;
using LibraryManagement.Domain.Interfaces.Repositories;

namespace LibraryManagement.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _uow;
    private readonly IJwtService _jwtService;
    private readonly IPasswordService _passwordService;

    public AuthService(IUnitOfWork uow, IJwtService jwtService, IPasswordService passwordService)
    {
        _uow = uow;
        _jwtService = jwtService;
        _passwordService = passwordService;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await _uow.Users.GetByUsernameAsync(request.UsernameOrEmail, ct)
                ?? await _uow.Users.GetByEmailAsync(request.UsernameOrEmail, ct)
                ?? throw new UnauthorizedException("Invalid username or password.");

        if (!user.IsActive)
            throw new UnauthorizedException("Your account has been deactivated. Contact an administrator.");

        if (!_passwordService.VerifyPassword(request.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid username or password.");

        user.RecordLogin();
        await _uow.Users.UpdateAsync(user, ct);
        await _uow.SaveChangesAsync(ct);

        var token = _jwtService.GenerateToken(user);

        return new LoginResponse(
            AccessToken: token,
            TokenType: "Bearer",
            ExpiresIn: _jwtService.GetExpirationMinutes() * 60,
            UserId: user.Id,
            Username: user.Username,
            Email: user.Email,
            Role: user.Role.Name,
            FullName: user.FullName
        );
    }
}
