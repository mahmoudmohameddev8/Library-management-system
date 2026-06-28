namespace LibraryManagement.Application.DTOs.Auth;

public record LoginRequest(string UsernameOrEmail, string Password);

public record LoginResponse(
    string AccessToken,
    string TokenType,
    int ExpiresIn,
    Guid UserId,
    string Username,
    string Email,
    string Role,
    string FullName
);
