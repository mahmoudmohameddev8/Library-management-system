using FluentValidation;
using LibraryManagement.Application.DTOs.Members;

namespace LibraryManagement.Application.Validators;

public class CreateMemberRequestValidator : AbstractValidator<CreateMemberRequest>
{
    public CreateMemberRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().MaximumLength(150);

        RuleFor(x => x.LastName)
            .NotEmpty().MaximumLength(150);

        RuleFor(x => x.Email)
            .NotEmpty().EmailAddress().MaximumLength(256);

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(30).When(x => x.PhoneNumber is not null);

        RuleFor(x => x.MembershipDurationMonths)
            .InclusiveBetween(1, 60).WithMessage("Membership duration must be between 1 and 60 months.");

        RuleFor(x => x.MaxBorrowLimit)
            .InclusiveBetween(1, 20).WithMessage("Borrow limit must be between 1 and 20.");
    }
}

public class UpdateMemberRequestValidator : AbstractValidator<UpdateMemberRequest>
{
    public UpdateMemberRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().MaximumLength(150);

        RuleFor(x => x.LastName)
            .NotEmpty().MaximumLength(150);

        RuleFor(x => x.Email)
            .NotEmpty().EmailAddress().MaximumLength(256);

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(30).When(x => x.PhoneNumber is not null);
    }
}
