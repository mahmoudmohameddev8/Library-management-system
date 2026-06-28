using LibraryManagement.Application.DTOs.Auth;

namespace LibraryManagement.Application.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
}
