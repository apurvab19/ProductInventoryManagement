using FluentValidation;
using ProductInventory.Application.DTOs.Products;

namespace ProductInventory.Application.Validators;

public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(request => request.ProductName)
            .NotEmpty()
            .MaximumLength(255);
    }
}
