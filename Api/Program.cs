using Api.Configurations;
using Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("StoreContext")
    ?? throw new InvalidOperationException("Connection string 'StoreContext' not found.");

builder.Services.AddDbContext<StoreContext>(options =>
{
    options.UseNpgsql(connectionString, p =>
    {
        p.MigrationsHistoryTable("__EFMigrationsHistory", "store");
    });
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.AddSwaggerGenWithAuthorize();

builder.AddUserAuthentication();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<StoreContext>();
await dbContext.Database.MigrateAsync();

await app.RunAsync();

public partial class Program;