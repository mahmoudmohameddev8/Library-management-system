using LibraryManagement.Application.Common.Exceptions;
using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Application.DTOs.Members;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Interfaces.Repositories;

namespace LibraryManagement.Application.Services;

public class MemberService : IMemberService
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public MemberService(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<IEnumerable<MemberDto>> GetAllAsync(CancellationToken ct = default)
    {
        var members = await _uow.Members.GetAllAsync(ct);
        return members.Select(MapToDto);
    }

    public async Task<MemberDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var member = await _uow.Members.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Member), id);
        return MapToDto(member);
    }

    public async Task<MemberDto> CreateAsync(CreateMemberRequest request, CancellationToken ct = default)
    {
        if (!await _uow.Members.IsEmailUniqueAsync(request.Email, cancellationToken: ct))
            throw new ConflictException($"A member with email '{request.Email}' already exists.");

        var membershipNumber = await _uow.Members.GenerateMembershipNumberAsync(ct);

        var member = Member.Create(
            membershipNumber,
            request.FirstName,
            request.LastName,
            request.Email,
            request.PhoneNumber,
            request.Address,
            request.MembershipDurationMonths,
            request.MaxBorrowLimit);

        member.CreatedAt = DateTime.UtcNow;
        member.CreatedBy = _currentUser.Username;

        await _uow.Members.AddAsync(member, ct);
        await _uow.SaveChangesAsync(ct);

        return MapToDto(member);
    }

    public async Task<MemberDto> UpdateAsync(Guid id, UpdateMemberRequest request, CancellationToken ct = default)
    {
        var member = await _uow.Members.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Member), id);

        if (!await _uow.Members.IsEmailUniqueAsync(request.Email, id, ct))
            throw new ConflictException($"Email '{request.Email}' is already in use by another member.");

        member.Update(request.FirstName, request.LastName, request.Email, request.PhoneNumber, request.Address);
        member.UpdatedAt = DateTime.UtcNow;
        member.UpdatedBy = _currentUser.Username;

        await _uow.Members.UpdateAsync(member, ct);
        await _uow.SaveChangesAsync(ct);

        return MapToDto(member);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var member = await _uow.Members.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Member), id);

        if (await _uow.Members.HasActiveBorrowingsAsync(id, ct))
            throw new ConflictException("Cannot delete a member who has active borrowings.");

        await _uow.Members.DeleteAsync(member, ct);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task SuspendAsync(Guid id, CancellationToken ct = default)
    {
        var member = await _uow.Members.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Member), id);

        member.Suspend();
        member.UpdatedAt = DateTime.UtcNow;
        member.UpdatedBy = _currentUser.Username;

        await _uow.Members.UpdateAsync(member, ct);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task RenewMembershipAsync(Guid id, int months = 12, CancellationToken ct = default)
    {
        var member = await _uow.Members.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Member), id);

        member.RenewMembership(months);
        member.UpdatedAt = DateTime.UtcNow;
        member.UpdatedBy = _currentUser.Username;

        await _uow.Members.UpdateAsync(member, ct);
        await _uow.SaveChangesAsync(ct);
    }

    private static MemberDto MapToDto(Member m) => new(
        m.Id, m.MembershipNumber,
        m.FirstName, m.LastName, m.FullName,
        m.Email, m.PhoneNumber, m.Address,
        m.Status.ToString(),
        m.MembershipStartDate, m.MembershipExpiryDate,
        m.MaxBorrowLimit);
}
