using FluentValidation;
using MinhaApiComSQLite.DTOs;

namespace MinhaApiComSQLite.Validators;

public class CreateCategoryValidator : AbstractValidator<CreateCategoryDto>
{
    public CreateCategoryValidator()
    {
        RuleFor(c => c.Name).NotEmpty().WithMessage("Category name is required");
    }
}
