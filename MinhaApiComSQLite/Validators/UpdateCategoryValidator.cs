using FluentValidation;
using MinhaApiComSQLite.DTOs;

namespace MinhaApiComSQLite.Validators;

public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryDto>
{
    public UpdateCategoryValidator()
    {
        RuleFor(dto => dto.Name).NotEmpty().WithMessage("Category name is required");
    }
}
