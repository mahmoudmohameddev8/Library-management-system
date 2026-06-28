using LibraryManagement.Application.DTOs.Members;

namespace LibraryManagement.Application.Services;

public interface IMemberService
{
    Task<IEnumerable<MemberDto>> GetAllAsync(CancellationToken ct = default);
    Task<MemberDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<MemberDto> CreateAsync(CreateMemberRequest request, CancellationToken ct = default);
    Task<MemberDto> UpdateAsync(Guid id, UpdateMemberRequest request, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task SuspendAsync(Guid id, CancellationToken ct = default);
    Task RenewMembershipAsync(Guid id, int months = 12, CancellationToken ct = default);
}
