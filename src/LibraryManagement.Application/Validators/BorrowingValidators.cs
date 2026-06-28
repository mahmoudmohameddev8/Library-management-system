using FluentValidation;
using LibraryManagement.Application.DTOs.Borrowing;

namespace LibraryManagement.Application.Validators;

public class BorrowBookRequestValidator : AbstractValidator<BorrowBookRequest>
{
    public BorrowBookRequestValidator()
    {
        RuleFor(x => x.BookId)
            .NotEqual(Guid.Empty).WithMessage("Book ID is required.");

        RuleFor(x => x.MemberId)
            .NotEqual(Guid.Empty).WithMessage("Member ID is required.");

        RuleFor(x => x.BorrowDurationDays)
            .InclusiveBetween(1, 90)
            .WithMessage("Borrow duration must be between 1 and 90 days.");
    }
}
