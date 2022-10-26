using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.AggregateModels.BuyerAggregate;
using OrderService.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Infrastructure.EntityConfigurations
{
    public class PaymentMethodEntityConfiguration : IEntityTypeConfiguration<PaymentMethod>
    {
        public void Configure(EntityTypeBuilder<PaymentMethod> builder)
        {
            builder.ToTable("PaymentMethods", OrderDbContext.DEFAULT_SCHEMA);

            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).ValueGeneratedOnAdd();

            builder.Ignore(i => i.DomainEvents);

            builder.Property<int>("BuyerId").IsRequired();

            builder.Property(x => x.CardHolderName)
                    .UsePropertyAccessMode(PropertyAccessMode.Field)
                    .HasColumnName("CardHolderName")
                    .HasMaxLength(200)
                    .IsRequired();

            builder.Property(x => x.Alias)
                    .UsePropertyAccessMode(PropertyAccessMode.Field)
                    .HasColumnName("Alias")
                    .HasMaxLength(200)
                    .IsRequired();

            builder.Property(x => x.CardNumber)
                    .UsePropertyAccessMode(PropertyAccessMode.Field)
                    .HasColumnName("CardNumber")
                    .HasMaxLength(25)
                    .IsRequired();

            builder.Property(x => x.Expiration)
                    .UsePropertyAccessMode(PropertyAccessMode.Field)
                    .HasColumnName("Expiration")
                    .HasMaxLength(25)
                    .IsRequired();

            builder.Property(x => x.CardTypeId)
                    .UsePropertyAccessMode(PropertyAccessMode.Field)
                    .HasColumnName("CardTypeId")
                    .IsRequired();

            builder.HasOne(p => p.CardType)
                .WithMany()
                .HasForeignKey(i => i.CardTypeId);
        }
    }
}
