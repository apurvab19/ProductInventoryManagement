using FluentValidation;
using ProductInventory.Application.DTOs.Products;

namespace ProductInventory.Application.Validators;

public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(request => request.ProductName)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(request => request.Quantity)
            .GreaterThanOrEqualTo(0);
    }
}
