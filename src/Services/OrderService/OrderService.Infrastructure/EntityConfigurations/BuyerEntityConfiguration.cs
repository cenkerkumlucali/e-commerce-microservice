using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.AggregateModels.BuyerAggregate;
using OrderService.Infrastructure.Context;

namespace OrderService.Infrastructure.EntityConfigurations;

public class BuyerEntityConfiguration : IEntityTypeConfiguration<Buyer>
{
    public void Configure(EntityTypeBuilder<Buyer> builder)
    {
        builder.ToTable("Buyers", OrderDbContext.DEFAULT_SCHEMA);

        builder.HasKey(b => b.Id);

        builder.Property(o => o.Id).ValueGeneratedOnAdd();

        builder.Ignore(i => i.DomainEvents);

        builder.Property(x => x.Name).HasColumnName("Name").HasColumnType("varchar").HasMaxLength(100);

        builder.HasMany(x => x.PaymentMethods)
            .WithOne()
            .HasForeignKey(x => x.Id)
            .OnDelete(DeleteBehavior.Cascade);

        var navigation = builder.Metadata.FindNavigation(nameof(Buyer.PaymentMethods));

        navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}