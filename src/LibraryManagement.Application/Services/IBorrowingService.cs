using LibraryManagement.Application.DTOs.Borrowing;

namespace LibraryManagement.Application.Services;

public interface IBorrowingService
{
    Task<BorrowingTransactionDto> BorrowAsync(BorrowBookRequest request, CancellationToken ct = default);
    Task<BorrowingTransactionDto> ReturnAsync(Guid transactionId, ReturnBookRequest request, CancellationToken ct = default);
    Task<IEnumerable<BorrowingTransactionDto>> GetByMemberAsync(Guid memberId, CancellationToken ct = default);
    Task<IEnumerable<BorrowingTransactionDto>> GetOverdueAsync(CancellationToken ct = default);
    Task<IEnumerable<BorrowingTransactionDto>> GetAllActiveAsync(CancellationToken ct = default);
}
