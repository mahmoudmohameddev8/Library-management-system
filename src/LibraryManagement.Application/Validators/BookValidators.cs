using FluentValidation;
using LibraryManagement.Application.DTOs.Books;

namespace LibraryManagement.Application.Validators;

public class CreateBookRequestValidator : AbstractValidator<CreateBookRequest>
{
    public CreateBookRequestValidator()
    {
        RuleFor(x => x.ISBN)
            .NotEmpty().WithMessage("ISBN is required.")
            .MaximumLength(20)
            .Must(IsValidIsbn).WithMessage("ISBN must be 10 or 13 digits (hyphens allowed).");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(500);

        RuleFor(x => x.TotalCopies)
            .GreaterThanOrEqualTo(0).WithMessage("Total copies cannot be negative.");

        RuleFor(x => x.PublisherId)
            .NotEqual(Guid.Empty).WithMessage("Publisher is required.");

        RuleFor(x => x.LanguageId)
            .NotEqual(Guid.Empty).WithMessage("Language is required.");

        RuleFor(x => x.CategoryId)
            .NotEqual(Guid.Empty).WithMessage("Category is required.");

        RuleFor(x => x.Authors)
            .NotNull().NotEmpty().WithMessage("At least one author is required.");

        RuleFor(x => x.Authors)
            .Must(a => a.Count(x => x.IsPrimaryAuthor) <= 1)
            .WithMessage("Only one primary author is allowed.")
            .When(x => x.Authors != null && x.Authors.Any());

        RuleFor(x => x.PageCount)
            .GreaterThan(0).WithMessage("Page count must be greater than 0.")
            .When(x => x.PageCount.HasValue);
    }

    private static bool IsValidIsbn(string isbn)
    {
        var digits = isbn.Replace("-", "").Replace(" ", "");
        return digits.Length is 10 or 13 && digits.All(char.IsDigit);
    }
}

public class UpdateBookRequestValidator : AbstractValidator<UpdateBookRequest>
{
    public UpdateBookRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(500);

        RuleFor(x => x.TotalCopies)
            .GreaterThanOrEqualTo(0).WithMessage("Total copies cannot be negative.");

        RuleFor(x => x.PublisherId)
            .NotEqual(Guid.Empty).WithMessage("Publisher is required.");

        RuleFor(x => x.LanguageId)
            .NotEqual(Guid.Empty).WithMessage("Language is required.");

        RuleFor(x => x.CategoryId)
            .NotEqual(Guid.Empty).WithMessage("Category is required.");

        RuleFor(x => x.PageCount)
            .GreaterThan(0).WithMessage("Page count must be greater than 0.")
            .When(x => x.PageCount.HasValue);
    }
}
