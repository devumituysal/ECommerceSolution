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
    internal class ProductImageEntityConfiguration : IEntityTypeConfiguration<ProductImageEntity>
    {
        public void Configure(EntityTypeBuilder<ProductImageEntity> builder)
        {
            builder.HasKey(pi => pi.Id);
            builder.Property(pi => pi.ProductId)
                .IsRequired();
            builder.Property(pi => pi.Url)
                .IsRequired()
                .HasMaxLength(250);
            builder.Property(pi => pi.CreatedAt)
                .IsRequired();

            builder.HasOne(pi => pi.Product)
                .WithMany(x => x.Images)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
