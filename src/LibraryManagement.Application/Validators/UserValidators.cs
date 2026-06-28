using FluentValidation;
using LibraryManagement.Application.DTOs.Users;

namespace LibraryManagement.Application.Validators;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().MinimumLength(3).MaximumLength(100)
            .Matches("^[a-zA-Z0-9_.-]+$")
            .WithMessage("Username can only contain letters, numbers, dots, dashes, and underscores.");

        RuleFor(x => x.Email)
            .NotEmpty().EmailAddress().MaximumLength(256);

        RuleFor(x => x.Password)
            .NotEmpty().MinimumLength(8).MaximumLength(100)
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(150);

        RuleFor(x => x.RoleId)
            .NotEqual(Guid.Empty).WithMessage("A valid role must be selected.");
    }
}

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
    }
}

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword).NotEmpty();

        RuleFor(x => x.NewPassword)
            .NotEmpty().MinimumLength(8)
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.")
            .NotEqual(x => x.CurrentPassword).WithMessage("New password must differ from the current password.");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.NewPassword).WithMessage("Passwords do not match.");
    }
}
