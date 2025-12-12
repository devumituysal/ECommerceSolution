using App.Data.Configurations;
using App.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Data.Contexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<UserEntity> Users { get; set; }
        public DbSet<RoleEntity> Roles { get; set; }
        public DbSet<ProductEntity> Products { get; set; }
        public DbSet<ProductImageEntity> ProductImages { get; set; }
        public DbSet<ProductCommentEntity> ProductComments { get; set; }
        public DbSet<OrderEntity> Orders { get; set; }
        public DbSet<OrderItemEntity> OrderItems { get; set; }
        public DbSet<CategoryEntity> Categories { get; set; }
        public DbSet<CartItemEntity> CartItems { get; set; }
        public DbSet<ContactMessageEntity> ContactMessages { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CartItemEntityConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryEntityConfiguration());
            modelBuilder.ApplyConfiguration(new OrderEntityConfiguration());
            modelBuilder.ApplyConfiguration(new OrderItemEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ProductCommentEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ProductEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ProductImageEntityConfiguration());
            modelBuilder.ApplyConfiguration(new RoleEntityConfiguration());
            modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ContactMessageEntityConfiguration());



            modelBuilder.Entity<RoleEntity>().HasData(
                new RoleEntity() { Id = 1, Name = "admin", CreatedAt = DateTime.UtcNow },
                new RoleEntity() { Id = 2, Name = "seller", CreatedAt = DateTime.UtcNow },
                new RoleEntity() { Id = 3, Name = "buyer", CreatedAt = DateTime.UtcNow }
            );

            modelBuilder.Entity<UserEntity>().HasData(
                new UserEntity() { Id = 1, FirstName = "admin", LastName = "admin", Email = "admin@gmail.com", Enabled = true, RoleId = 1, Password = "1234", CreatedAt = DateTime.UtcNow },
                new UserEntity() { Id = 2, FirstName = "seller", LastName = "seller", Email = "seller@gmail.com", Enabled = true, RoleId = 2, Password = "1234", CreatedAt = DateTime.UtcNow }
            );

            modelBuilder.Entity<CategoryEntity>().HasData(
                    new CategoryEntity { Id = 1, Name = "Electronics", Color = "Blue", IconCssClass = "fa fa-fw fa-bolt", CreatedAt = DateTime.UtcNow },
                    new CategoryEntity { Id = 2, Name = "Clothing", Color = "Red", IconCssClass = "fa fa-fw fa-shopping-bag", CreatedAt = DateTime.UtcNow },
                    new CategoryEntity { Id = 3, Name = "Home", Color = "Green", IconCssClass = "fa fa-fw fa-home", CreatedAt = DateTime.UtcNow },
                    new CategoryEntity { Id = 4, Name = "Books", Color = "Orange", IconCssClass = "fa fa-fw fa-book", CreatedAt = DateTime.UtcNow },
                    new CategoryEntity { Id = 5, Name = "Health", Color = "Purple", IconCssClass = "fa fa-fw fa-heart", CreatedAt = DateTime.Now },
                    new CategoryEntity { Id = 6, Name = "Sports", Color = "Yellow", IconCssClass = "fa fa-fw fa-trophy", CreatedAt = DateTime.UtcNow },
                    new CategoryEntity { Id = 7, Name = "Toys", Color = "Pink", IconCssClass = "fa fa-fw fa-child", CreatedAt = DateTime.UtcNow },
                    new CategoryEntity { Id = 8, Name = "Automotive", Color = "Grey", IconCssClass = "fa fa-fw fa-car", CreatedAt = DateTime.UtcNow },
                    new CategoryEntity { Id = 9, Name = "Furniture", Color = "Brown", IconCssClass = "fa fa-fw fa-tree", CreatedAt = DateTime.UtcNow },
                    new CategoryEntity { Id = 10, Name = "Food", Color = "Black", IconCssClass = "fa fa-fw fa-cutlery", CreatedAt = DateTime.UtcNow }
            );

            modelBuilder.Entity<ProductEntity>().HasData(
                new ProductEntity
                {
                    Id = 1,
                    SellerId = 1,
                    CategoryId = 1, // CategoryId = 1'in DB'de olduğundan emin olundu
                    Name = "Test Product",
                    Price = 149.99m,
                    Details = "This is a test product used for checking the Product Detail page.",
                    StockAmount = 10,
                    Enabled = true,
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
}
