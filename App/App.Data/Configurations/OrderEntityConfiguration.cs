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
    internal class OrderEntityConfiguration : IEntityTypeConfiguration<OrderEntity>
    {
        public void Configure(EntityTypeBuilder<OrderEntity> builder)
        {
            builder.HasKey(o => o.Id);
            builder.Property(o => o.UserId)
                .IsRequired();
            builder.Property(o => o.OrderCode)
                .IsRequired()
                .HasMaxLength(250);
            builder.Property(o => o.Address)
                .IsRequired()
                .HasMaxLength(250);
            builder.Property(o => o.CreatedAt)
                .IsRequired();

            builder.HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
