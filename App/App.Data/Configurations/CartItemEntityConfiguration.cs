using App.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Data.Configurations
{
    internal class CartItemEntityConfiguration : IEntityTypeConfiguration<CartItemEntity>
    {
        public void Configure(EntityTypeBuilder<CartItemEntity> builder)
        {
            builder.HasKey(ci => ci.Id);
            builder.Property(ci => ci.UserId)
                .IsRequired();
            builder.Property(ci => ci.ProductId)
                .IsRequired();
            builder.Property(ci => ci.Quantity)
                .IsRequired()
                .HasDefaultValue(1);
            builder.Property(ci => ci.CreatedAt)
                .IsRequired();

            builder.HasOne(ci => ci.User)
                .WithMany()
                .HasForeignKey(ci => ci.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
