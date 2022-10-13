using CatalogService.Api.Core.Domain;
using CatalogService.Api.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Api.Infrastructure.EntityConfigurations;

public class CatalogTypeEntityTypeConfiguration: IEntityTypeConfiguration<CatalogType>
{
    public void Configure(EntityTypeBuilder<CatalogType> builder)
    {
        builder.ToTable("CatalogType", CatalogContext.DEFAULT_SCHEMA);
        
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Id)
            .UseHiLo("catalog_type_hilo")
            .IsRequired();
        
        builder.Property(c => c.Type)
            .IsRequired()
            .HasMaxLength(100);
    }
}