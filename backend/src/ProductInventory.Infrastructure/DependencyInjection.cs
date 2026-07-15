using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductInventory.Application.Common.Interfaces;
using ProductInventory.Application.Common.Models;
using ProductInventory.Application.Interfaces;
using ProductInventory.Domain.Entities;
using ProductInventory.Infrastructure.Data;
using ProductInventory.Infrastructure.Data.Repositories;
using ProductInventory.Infrastructure.Identity;

namespace ProductInventory.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(
            "DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' was not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        var jwtSection = configuration.GetSection("Jwt");

        services.Configure<JwtSettings>(options =>
        {
            options.Issuer = jwtSection["Issuer"] ?? string.Empty;
            options.Audience = jwtSection["Audience"] ?? string.Empty;
            options.SecretKey = jwtSection["SecretKey"] ?? string.Empty;

            options.AccessTokenMinutes =
                int.TryParse(jwtSection["AccessTokenMinutes"], out var minutes)
                    ? minutes
                    : 15;

            options.RefreshTokenDays =
                int.TryParse(jwtSection["RefreshTokenDays"], out var days)
                    ? days
                    : 7;
        });

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IPasswordHasher<AppUser>, PasswordHasher<AppUser>>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}

