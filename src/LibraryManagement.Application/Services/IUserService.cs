using LibraryManagement.Application.DTOs.Users;

namespace LibraryManagement.Application.Services;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken ct = default);
    Task<UserDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<UserDto> CreateAsync(CreateUserRequest request, CancellationToken ct = default);
    Task<UserDto> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken ct = default);
    Task DeactivateAsync(Guid id, CancellationToken ct = default);
    Task ChangePasswordAsync(Guid id, ChangePasswordRequest request, CancellationToken ct = default);
}
