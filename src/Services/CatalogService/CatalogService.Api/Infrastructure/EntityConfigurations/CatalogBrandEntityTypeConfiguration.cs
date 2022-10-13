using CatalogService.Api.Core.Domain;
using CatalogService.Api.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Api.Infrastructure.EntityConfigurations;

public class CatalogBrandEntityTypeConfiguration : IEntityTypeConfiguration<CatalogBrand>
{
    public void Configure(EntityTypeBuilder<CatalogBrand> builder)
    {
        builder.ToTable("CatalogBrand", CatalogContext.DEFAULT_SCHEMA);
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .UseHiLo("catalog_brand_hilo")
            .IsRequired();
        builder.Property(c => c.Brand)
            .IsRequired()
            .HasMaxLength(100);
    }
}