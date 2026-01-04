using App.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Data.Seeds
{
    public static class ProductSeed
    {
        public static List<ProductEntity> Data => new()
        {
            // ================= ELECTRONİCS (1)
            new ProductEntity { Id = 1, SellerId = 2, CategoryId = 1, Name = "Wireless Bluetooth Headphones", Price = 149.99m, Details = "Over-ear wireless Bluetooth headphones with noise cancellation and 20 hours battery life.", StockAmount = 10, Enabled = true, CreatedAt = DateTime.UtcNow},
            new ProductEntity { Id = 2, SellerId = 2, CategoryId = 1, Name = "Lenovo IdeaPad 3 Laptop", Price = 18500, StockAmount = 10, Enabled = true, Details = "15.6 inch i5 laptop", CreatedAt = new DateTime(2024,1,1)},
            new ProductEntity { Id = 3, SellerId = 3, CategoryId = 1, Name = "Apple MacBook Air M1", Price = 29500, StockAmount = 6, Enabled = true, Details = "13 inch M1 chip", CreatedAt = new DateTime(2024,1,1)},
            new ProductEntity { Id = 4, SellerId = 4, CategoryId = 1, Name = "Samsung Galaxy S23", Price = 26500, StockAmount = 8, Enabled = true, Details = "Android smartphone", CreatedAt = new DateTime(2024,1,1)},
            new ProductEntity { Id = 5, SellerId = 5, CategoryId = 1, Name = "Apple iPad 10th Gen", Price = 14500, StockAmount = 7, Enabled = true, Details = "10.9 inch tablet", CreatedAt = new DateTime(2024,1,1)},
            new ProductEntity { Id = 6, SellerId = 6, CategoryId = 1, Name = "Logitech MX Master 3 Mouse", Price = 2200, StockAmount = 20, Enabled = true, Details = "Wireless mouse", CreatedAt = new DateTime(2024,1,1)},

            // ================= CLOTHING (2)
            new ProductEntity { Id = 7, SellerId = 2, CategoryId = 2, Name = "Basic White T-Shirt", Price = 299, StockAmount = 40, Enabled = true, Details = "100% cotton", CreatedAt = new DateTime(2024,1,1)},
            new ProductEntity { Id = 8, SellerId = 3, CategoryId = 2, Name = "Slim Fit Jeans", Price = 899, StockAmount = 25, Enabled = true, Details = "Dark blue denim", CreatedAt = new DateTime(2024,1,1)},
            new ProductEntity { Id = 9, SellerId = 4, CategoryId = 2, Name = "Hooded Sweatshirt", Price = 1200, StockAmount = 18, Enabled = true, Details = "Winter hoodie", CreatedAt = new DateTime(2024,1,1)},
            new ProductEntity { Id = 10, SellerId = 5, CategoryId = 2, Name = "Leather Jacket", Price = 4200, StockAmount = 6, Enabled = true, Details = "Genuine leather", CreatedAt = new DateTime(2024,1,1)},
            new ProductEntity { Id = 11, SellerId = 6, CategoryId = 2, Name = "Running Shorts", Price = 450, StockAmount = 30, Enabled = true, Details = "Breathable fabric", CreatedAt = new DateTime(2024,1,1)},

            // ================= HOME (3)
            new ProductEntity { Id = 12, SellerId = 2, CategoryId = 3, Name = "Wooden Dining Table", Price = 9500, StockAmount = 3, Enabled = true, Details = "Solid oak table", CreatedAt = new DateTime(2024,1,1)},
            new ProductEntity { Id = 13, SellerId = 3, CategoryId = 3, Name = "Ergonomic Office Chair", Price = 3200, StockAmount = 7, Enabled = true, Details = "Adjustable chair", CreatedAt = new DateTime(2024,1,1)},
            new ProductEntity { Id = 14, SellerId = 4, CategoryId = 3, Name = "LED Table Lamp", Price = 850, StockAmount = 20, Enabled = true, Details = "Warm light", CreatedAt = new DateTime(2024,1,1)},
            new ProductEntity { Id = 15, SellerId = 5, CategoryId = 3, Name = "Blackout Curtain Set", Price = 1250, StockAmount = 15, Enabled = true, Details = "Sun blocking", CreatedAt = new DateTime(2024,1,1)},
            new ProductEntity { Id = 16, SellerId = 6, CategoryId = 3, Name = "Silent Wall Clock", Price = 600, StockAmount = 18, Enabled = true, Details = "No ticking sound", CreatedAt = new DateTime(2024,1,1)},

            // ================= BOOKS (4)
            new ProductEntity { Id = 17, SellerId = 2, CategoryId = 4, Name = "Clean Code", Price = 650, StockAmount = 14, Enabled = true, Details = "Robert C. Martin", CreatedAt = new DateTime(2024,1,1)},
            new ProductEntity { Id = 18, SellerId = 3, CategoryId = 4, Name = "Design Patterns", Price = 720, StockAmount = 10, Enabled = true, Details = "GoF patterns", CreatedAt = new DateTime(2024,1,1)},
            new ProductEntity { Id = 19, SellerId = 4, CategoryId = 4, Name = "1984 – George Orwell", Price = 180, StockAmount = 40, Enabled = true, Details = "Dystopian novel", CreatedAt = new DateTime(2024,1,1)},
            new ProductEntity { Id = 20, SellerId = 5, CategoryId = 4, Name = "Sapiens", Price = 320, StockAmount = 22, Enabled = true, Details = "Human history", CreatedAt = new DateTime(2024,1,1)},
            new ProductEntity { Id = 21, SellerId = 6, CategoryId = 4, Name = "The Pragmatic Programmer", Price = 680, StockAmount = 16, Enabled = true, Details = "Software craftsmanship", CreatedAt = new DateTime(2024,1,1)},

            // ================= HEALTH (5)
            new ProductEntity { Id = 22, SellerId = 2, CategoryId = 5, Name = "Vitamin C 1000mg", Price = 220, StockAmount = 50, Enabled = true, Details = "Immune support", CreatedAt = new DateTime(2024,1,1)},
            new ProductEntity { Id = 23, SellerId = 3, CategoryId = 5, Name = "Protein Powder", Price = 950, StockAmount = 18, Enabled = true, Details = "Whey protein", CreatedAt = new DateTime(2024,1,1)},
            new ProductEntity { Id = 24, SellerId = 4, CategoryId = 5, Name = "Digital Thermometer", Price = 180, StockAmount = 30, Enabled = true, Details = "Fast measurement", CreatedAt = new DateTime(2024,1,1)},
            new ProductEntity { Id = 25, SellerId = 5, CategoryId = 5, Name = "Hot Water Bottle", Price = 140, StockAmount = 25, Enabled = true, Details = "Rubber bottle", CreatedAt = new DateTime(2024,1,1)},
            new ProductEntity { Id = 26, SellerId = 6, CategoryId = 5, Name = "Orthopedic Pillow", Price = 750, StockAmount = 12, Enabled = true, Details = "Neck support", CreatedAt = new DateTime(2024,1,1)},

            // ================= SPORTS (6)
            new ProductEntity { Id = 27, SellerId = 2, CategoryId = 6, Name = "Football Ball", Price = 350, StockAmount = 25, Enabled = true, Details = "Size 5", CreatedAt = new DateTime(2024,1,1)},
            new ProductEntity { Id = 28, SellerId = 3, CategoryId = 6, Name = "Yoga Mat", Price = 420, StockAmount = 30, Enabled = true, Details = "Non-slip", CreatedAt = new DateTime(2024,1,1)},
            new ProductEntity { Id = 29, SellerId = 4, CategoryId = 6, Name = "Adjustable Dumbbell Set", Price = 2200, StockAmount = 6, Enabled = true, Details = "20kg set", CreatedAt = new DateTime(2024,1,1)},
            new ProductEntity { Id = 30, SellerId = 5, CategoryId = 6, Name = "Jump Rope", Price = 120, StockAmount = 45, Enabled = true, Details = "Speed rope", CreatedAt = new DateTime(2024,1,1)},
            new ProductEntity { Id = 31, SellerId = 6, CategoryId = 6, Name = "Resistance Band Set", Price = 260, StockAmount = 35, Enabled = true, Details = "5 levels", CreatedAt = new DateTime(2024,1,1)},

            // ================= TOYS (7)
            new ProductEntity { Id = 32, SellerId = 2, CategoryId = 7, Name = "LEGO City Set", Price = 850, StockAmount = 14, Enabled = true, Details = "Creative toys", CreatedAt = new DateTime(2024,1,1)},
            new ProductEntity { Id = 33, SellerId = 3, CategoryId = 7, Name = "Remote Control Car", Price = 1250, StockAmount = 10, Enabled = true, Details = "Rechargeable", CreatedAt = new DateTime(2024,1,1)},
            new ProductEntity { Id = 34, SellerId = 4, CategoryId = 7, Name = "Plush Teddy Bear", Price = 280, StockAmount = 30, Enabled = true, Details = "Soft toy", CreatedAt = new DateTime(2024,1,1)},
            new ProductEntity { Id = 35, SellerId = 5, CategoryId = 7, Name = "Puzzle 1000 Pieces", Price = 320, StockAmount = 20, Enabled = true, Details = "Landscape puzzle", CreatedAt = new DateTime(2024,1,1)},
            new ProductEntity { Id = 36, SellerId = 6, CategoryId = 7, Name = "Toy Train Set", Price = 980, StockAmount = 8, Enabled = true, Details = "Battery powered", CreatedAt = new DateTime(2024,1,1)},

            // ================= AUTOMOTIVE (8)
            new ProductEntity { Id = 37, SellerId = 2, CategoryId = 8, Name = "Car Vacuum Cleaner", Price = 750, StockAmount = 15, Enabled = true, Details = "12V vacuum", CreatedAt = new DateTime(2024,1,1)},
            new ProductEntity { Id = 38, SellerId = 3, CategoryId = 8, Name = "Engine Oil 5W-30", Price = 620, StockAmount = 22, Enabled = true, Details = "Synthetic oil", CreatedAt = new DateTime(2024,1,1)},
            new ProductEntity { Id = 39, SellerId = 4, CategoryId = 8, Name = "Car Battery 60Ah", Price = 2800, StockAmount = 6, Enabled = true, Details = "Maintenance free", CreatedAt = new DateTime(2024,1,1)},
            new ProductEntity { Id = 40, SellerId = 5, CategoryId = 8, Name = "Windshield Wipers", Price = 180, StockAmount = 35, Enabled = true, Details = "Universal fit", CreatedAt = new DateTime(2024,1,1)},
            new ProductEntity { Id = 41, SellerId = 6, CategoryId = 8, Name = "Car Phone Holder", Price = 220, StockAmount = 40, Enabled = true, Details = "Magnetic mount", CreatedAt = new DateTime(2024,1,1)},

            // ================= FURNITURE (9)
            new ProductEntity { Id = 42, SellerId = 2, CategoryId = 9, Name = "3-Seater Sofa", Price = 18500, StockAmount = 2, Enabled = true, Details = "Fabric sofa", CreatedAt = new DateTime(2024,1,1)},
            new ProductEntity { Id = 43, SellerId = 3, CategoryId = 9, Name = "Wardrobe Cabinet", Price = 12500, StockAmount = 3, Enabled = true, Details = "Sliding doors", CreatedAt = new DateTime(2024,1,1)},
            new ProductEntity { Id = 44, SellerId = 4, CategoryId = 9, Name = "TV Stand", Price = 4200, StockAmount = 6, Enabled = true, Details = "Wooden stand", CreatedAt = new DateTime(2024,1,1)},
            new ProductEntity { Id = 45, SellerId = 5, CategoryId = 9, Name = "Nightstand", Price = 1800, StockAmount = 10, Enabled = true, Details = "Minimal design", CreatedAt = new DateTime(2024,1,1)},
            new ProductEntity { Id = 46, SellerId = 6, CategoryId = 9, Name = "Bookshelf", Price = 3600, StockAmount = 7, Enabled = true, Details = "5 shelves", CreatedAt = new DateTime(2024,1,1)},

            // ================= FOOD (10)
            new ProductEntity { Id = 47, SellerId = 2, CategoryId = 10, Name = "Organic Red Apples (1kg)", Price = 65, StockAmount = 100, Enabled = true, Details = "Fresh apples", CreatedAt = new DateTime(2024,1,1)},
            new ProductEntity { Id = 48, SellerId = 3, CategoryId = 10, Name = "Natural Honey (500g)", Price = 180, StockAmount = 35, Enabled = true, Details = "No additives", CreatedAt = new DateTime(2024,1,1)},
            new ProductEntity { Id = 49, SellerId = 4, CategoryId = 10, Name = "Aged White Cheese", Price = 210, StockAmount = 20, Enabled = true, Details = "Traditional cheese", CreatedAt = new DateTime(2024,1,1)},
            new ProductEntity { Id = 50, SellerId = 5, CategoryId = 10, Name = "Extra Virgin Olive Oil", Price = 320, StockAmount = 25, Enabled = true, Details = "Cold pressed", CreatedAt = new DateTime(2024,1,1)},
            new ProductEntity { Id = 51, SellerId = 6, CategoryId = 10, Name = "Whole Wheat Bread", Price = 35, StockAmount = 60, Enabled = true, Details = "Daily baked", CreatedAt = new DateTime(2024,1,1)}

        };
    }
}
