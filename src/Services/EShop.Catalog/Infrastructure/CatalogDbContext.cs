using EShop.Catalog.Models;
using Microsoft.EntityFrameworkCore;

namespace EShop.Catalog.Infrastructure;

public class CatalogDbContext(DbContextOptions<CatalogDbContext> options) : DbContext(options)
{

    public DbSet<CatalogBrand> CatalogBrands { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<CatalogCategory> CatalogCategories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);
        modelBuilder.HasDefaultSchema("catalog");
        base.OnModelCreating(modelBuilder);
    }
}