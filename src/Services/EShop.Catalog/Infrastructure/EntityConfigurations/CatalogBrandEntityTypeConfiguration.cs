using EShop.Catalog.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EShop.Catalog.Infrastructure.EntityConfigurations;

public sealed class CatalogBrandEntityTypeConfiguration : IEntityTypeConfiguration<CatalogBrand>
{
    public void Configure(EntityTypeBuilder<CatalogBrand> builder)
    {
        builder.ToTable("CatalogBrands");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Brand)
            .HasMaxLength(100)
            .IsRequired(true);
    }
}