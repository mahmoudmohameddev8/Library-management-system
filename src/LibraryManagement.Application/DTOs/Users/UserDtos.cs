namespace LibraryManagement.Application.DTOs.Users;

public record UserDto(
    Guid Id,
    string Username,
    string Email,
    string FirstName,
    string LastName,
    string FullName,
    string RoleName,
    Guid RoleId,
    bool IsActive,
    DateTime? LastLoginAt,
    DateTime CreatedAt
);

public record CreateUserRequest(
    string Username,
    string Email,
    string Password,
    string FirstName,
    string LastName,
    Guid RoleId
);

public record UpdateUserRequest(
    string FirstName,
    string LastName,
    string Email,
    Guid? RoleId = null
);

public record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword,
    string ConfirmPassword
);
