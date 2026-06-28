using LibraryManagement.Application.Common.Exceptions;
using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Application.DTOs.Users;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Interfaces.Repositories;

namespace LibraryManagement.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;
    private readonly IPasswordService _passwordService;

    public UserService(IUnitOfWork uow, ICurrentUserService currentUser, IPasswordService passwordService)
    {
        _uow = uow;
        _currentUser = currentUser;
        _passwordService = passwordService;
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken ct = default)
    {
        var users = await _uow.Users.GetAllAsync(ct);
        return users.Select(MapToDto);
    }

    public async Task<UserDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var user = await _uow.Users.GetWithRoleAsync(id, ct)
            ?? throw new NotFoundException(nameof(User), id);
        return MapToDto(user);
    }

    public async Task<UserDto> CreateAsync(CreateUserRequest request, CancellationToken ct = default)
    {
        if (!await _uow.Users.IsUsernameUniqueAsync(request.Username, cancellationToken: ct))
            throw new ConflictException($"Username '{request.Username}' is already taken.");

        if (!await _uow.Users.IsEmailUniqueAsync(request.Email, cancellationToken: ct))
            throw new ConflictException($"Email '{request.Email}' is already in use.");

        var passwordHash = _passwordService.HashPassword(request.Password);
        var user = User.Create(request.Username, request.Email, passwordHash,
                               request.FirstName, request.LastName, request.RoleId);

        user.CreatedAt = DateTime.UtcNow;
        user.CreatedBy = _currentUser.Username;

        await _uow.Users.AddAsync(user, ct);
        await _uow.SaveChangesAsync(ct);

        return MapToDto(await _uow.Users.GetWithRoleAsync(user.Id, ct) ?? user);
    }

    public async Task<UserDto> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken ct = default)
    {
        var user = await _uow.Users.GetWithRoleAsync(id, ct)
            ?? throw new NotFoundException(nameof(User), id);

        if (!await _uow.Users.IsEmailUniqueAsync(request.Email, id, ct))
            throw new ConflictException($"Email '{request.Email}' is already in use.");

        user.UpdateProfile(request.FirstName, request.LastName, request.Email);

        if (request.RoleId.HasValue)
            user.ChangeRole(request.RoleId.Value);

        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedBy = _currentUser.Username;

        await _uow.Users.UpdateAsync(user, ct);
        await _uow.SaveChangesAsync(ct);

        return MapToDto(await _uow.Users.GetWithRoleAsync(id, ct) ?? user);
    }

    public async Task DeactivateAsync(Guid id, CancellationToken ct = default)
    {
        var user = await _uow.Users.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(User), id);

        if (user.Id == _currentUser.UserId)
            throw new ConflictException("You cannot deactivate your own account.");

        user.Deactivate();
        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedBy = _currentUser.Username;

        await _uow.Users.UpdateAsync(user, ct);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task ChangePasswordAsync(Guid id, ChangePasswordRequest request, CancellationToken ct = default)
    {
        var user = await _uow.Users.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(User), id);

        if (!_passwordService.VerifyPassword(request.CurrentPassword, user.PasswordHash))
            throw new UnauthorizedException("Current password is incorrect.");

        if (request.NewPassword != request.ConfirmPassword)
            throw new ValidationException(new[] { "New password and confirmation do not match." });

        user.ChangePassword(_passwordService.HashPassword(request.NewPassword));
        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedBy = _currentUser.Username;

        await _uow.Users.UpdateAsync(user, ct);
        await _uow.SaveChangesAsync(ct);
    }

    private static UserDto MapToDto(User u) => new(
        u.Id, u.Username, u.Email,
        u.FirstName, u.LastName, u.FullName,
        u.Role?.Name ?? string.Empty, u.RoleId,
        u.IsActive, u.LastLoginAt, u.CreatedAt);
}
