using Microsoft.EntityFrameworkCore;
using ProductInventory.API.Extensions;
using ProductInventory.API.Middleware;
using ProductInventory.Application;
using ProductInventory.Infrastructure;
using ProductInventory.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddApiServices(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var database = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();

    await database.Database.MigrateAsync();
}

await DatabaseSeeder.SeedAdminAsync(
    app.Services,
    app.Configuration);

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => Results.Ok(new
{
    service = "Product Inventory API",
    status = "Running"
}));

app.Run();

public partial class Program;
