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
    internal class FavoriteEntityConfiguration : IEntityTypeConfiguration<FavoriteEntity>
    {
        public void Configure(EntityTypeBuilder<FavoriteEntity> builder)
        {
            builder.HasKey(fe => fe.Id);
            builder.Property(fe => fe.UserId)
                .IsRequired();
            builder.Property(fe => fe.ProductId)
                .IsRequired();

            builder.HasOne(fe => fe.User)
                .WithMany(u => u.Favorites)
                .HasForeignKey(fe => fe.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(fe => fe.Product)
                .WithMany(p => p.Favorites)
                .HasForeignKey(fe => fe.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(fe => new { fe.UserId, fe.ProductId })
                .IsUnique();
        }
    }
}
