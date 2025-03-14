using EShop.Catalog.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EShop.Catalog.Infrastructure.EntityConfigurations;

public class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(x => x.Slug);
        builder.Property(x => x.Slug)
            .HasMaxLength(150);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(5000);
         
        builder.Property(x => x.Price)
            .HasColumnType("decimal(15,2)");
 
        builder.OwnsMany(x => x.Medias, builder =>
        {
            builder.ToJson();

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Url)
                .IsRequired()
                .HasMaxLength(1098);
        });
    }

}