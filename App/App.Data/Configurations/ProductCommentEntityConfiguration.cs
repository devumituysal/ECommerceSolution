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
    internal class ProductCommentEntityConfiguration : IEntityTypeConfiguration<ProductCommentEntity>
    {
        public void Configure(EntityTypeBuilder<ProductCommentEntity> builder)
        {
            builder.HasKey(pc => pc.Id);
            builder.Property(pc => pc.ProductId).IsRequired();
            builder.Property(pc => pc.UserId).IsRequired();
            builder.Property(pc => pc.Text).IsRequired().HasMaxLength(500);
            builder.Property(pc => pc.StarCount).IsRequired().HasDefaultValue(3);
            builder.Property(pc => pc.IsConfirmed).IsRequired().HasDefaultValue(false);
            builder.Property(pc => pc.CreatedAt).IsRequired();

            builder.HasOne(pc => pc.Product)
                .WithMany()
                .HasForeignKey(pc => pc.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(pc => pc.User)
                .WithMany()
                .HasForeignKey(pc => pc.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
