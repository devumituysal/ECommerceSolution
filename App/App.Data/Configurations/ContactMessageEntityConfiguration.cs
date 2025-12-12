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
    internal class ContactMessageEntityConfiguration : IEntityTypeConfiguration<ContactMessageEntity>
    {
        public void Configure(EntityTypeBuilder<ContactMessageEntity> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
            builder.Property(e => e.Email).IsRequired().HasMaxLength(256);
            builder.Property(e => e.Message).IsRequired().HasMaxLength(1000);
            builder.Property(e => e.CreatedAt).IsRequired();
        }
    }
}
