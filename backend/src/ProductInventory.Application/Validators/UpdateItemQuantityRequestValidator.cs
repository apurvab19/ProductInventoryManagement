using FluentValidation;
using ProductInventory.Application.DTOs.Items;

namespace ProductInventory.Application.Validators;

public class UpdateItemQuantityRequestValidator
    : AbstractValidator<UpdateItemQuantityRequest>
{
    public UpdateItemQuantityRequestValidator()
    {
        RuleFor(request => request.Quantity)
            .GreaterThanOrEqualTo(0);
    }
}
