using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductInventory.Application.Common.Models;
using ProductInventory.Application.DTOs.Items;
using ProductInventory.Application.DTOs.Products;
using ProductInventory.Application.Interfaces;

namespace ProductInventory.API.Controllers;

[ApiController]
[Authorize]
[Route("api/products")]
public sealed class ProductsController(
    IProductService productService,
    IValidator<CreateProductRequest> createValidator,
    IValidator<UpdateProductRequest> updateValidator,
    IValidator<UpdateItemQuantityRequest> quantityValidator)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<ProductDto>>> GetAll(
        [FromQuery] ProductQueryParameters parameters,
        CancellationToken cancellationToken)
    {
        var products = await productService.GetPagedAsync(
            parameters,
            cancellationToken);

        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var product = await productService.GetByIdAsync(
            id,
            cancellationToken);

        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var validation = await createValidator.ValidateAsync(
            request,
            cancellationToken);

        if (!validation.IsValid)
        {
            return ValidationProblem(
                new ValidationProblemDetails(
                    ToValidationDictionary(validation)));
        }

        var product = await productService.CreateAsync(
            request,
            GetCurrentUsername(),
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = product.Id },
            product);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        var validation = await updateValidator.ValidateAsync(
            request,
            cancellationToken);

        if (!validation.IsValid)
        {
            return ValidationProblem(
                new ValidationProblemDetails(
                    ToValidationDictionary(validation)));
        }

        var updated = await productService.UpdateAsync(
            id,
            request,
            GetCurrentUsername(),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpPut("{id:int}/quantity")]
    public async Task<IActionResult> UpdateQuantity(
        int id,
        UpdateItemQuantityRequest request,
        CancellationToken cancellationToken)
    {
        var validation = await quantityValidator.ValidateAsync(
            request,
            cancellationToken);

        if (!validation.IsValid)
        {
            return ValidationProblem(
                new ValidationProblemDetails(
                    ToValidationDictionary(validation)));
        }

        var updated = await productService.UpdateQuantityAsync(
            id,
            request,
            GetCurrentUsername(),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var deleted = await productService.DeleteAsync(
            id,
            cancellationToken);

        return deleted ? NoContent() : NotFound();
    }

    private string GetCurrentUsername()
    {
        return User.Identity?.Name
            ?? throw new InvalidOperationException(
                "Authenticated username is unavailable.");
    }

    private static Dictionary<string, string[]> ToValidationDictionary(
        FluentValidation.Results.ValidationResult validation)
    {
        return validation.Errors
            .GroupBy(error => error.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group
                    .Select(error => error.ErrorMessage)
                    .ToArray());
    }
}
