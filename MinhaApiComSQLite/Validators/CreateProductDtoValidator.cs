using FluentValidation;
using MinhaApiComSQLite.DTOs;

namespace MinhaApiComSQLite.Validators;

public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(dto => dto.Name).NotEmpty().WithMessage("Product name is required");
        RuleFor(dto => dto.Price).NotEmpty().WithMessage("Product price is required");
        RuleFor(dto => dto.CategoryId).NotEmpty().WithMessage("Product category is required");
    }
}
