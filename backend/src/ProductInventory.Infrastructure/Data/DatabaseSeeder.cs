using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductInventory.Domain.Entities;

namespace ProductInventory.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAdminAsync(
        IServiceProvider services,
        IConfiguration configuration)
    {
        var username = configuration["SeedAdmin:Username"];
        var password = configuration["SeedAdmin:Password"];

        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        using var scope = services.CreateScope();

        var context =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var passwordHasher =
            scope.ServiceProvider.GetRequiredService<IPasswordHasher<AppUser>>();

        var user = await context.Users
            .FirstOrDefaultAsync(x => x.Username == username);

        if (user is null)
        {
            user = new AppUser
            {
                Username = username,
                Role = "Admin"
            };

            user.PasswordHash =
                passwordHasher.HashPassword(user, password);

            await context.Users.AddAsync(user);
        }
        else
        {
            user.Role = "Admin";
        }

        await context.SaveChangesAsync();
    }
}
