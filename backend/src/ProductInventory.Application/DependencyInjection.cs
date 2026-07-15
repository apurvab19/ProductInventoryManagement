using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ProductInventory.Application.Interfaces;
using ProductInventory.Application.Services;

namespace ProductInventory.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<IProductService, ProductService>();

        return services;
    }
}
