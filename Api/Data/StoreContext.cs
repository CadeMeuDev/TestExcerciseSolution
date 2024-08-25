using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Data;

public class StoreContext(DbContextOptions<StoreContext> options)
    : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("store");
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Product> Product { get; set; } = default!;
}