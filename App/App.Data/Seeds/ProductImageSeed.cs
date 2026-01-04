using App.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Data.Seeds
{
    public static class ProductImageSeed
    {
        public static List<ProductImageEntity> Data => new()
        {
             // ELECTRONICS
            new() { Id = 1, ProductId = 1, Url = "https://localhost:7132/uploads/seedImages/wireless-bluetooth-headphones.png" },
            new() { Id = 2, ProductId = 2, Url = "https://localhost:7132/uploads/seedImages/lenovo-ideapad-3-laptop.png" },
            new() { Id = 3, ProductId = 3, Url = "https://localhost:7132/uploads/seedImages/apple-macbook-air-m1.png" },
            new() { Id = 4, ProductId = 4, Url = "https://localhost:7132/uploads/seedImages/samsung-galaxy-s23.png" },
            new() { Id = 5, ProductId = 5, Url = "https://localhost:7132/uploads/seedImages/apple-ipad-10th-gen.png" },
            new() { Id = 6, ProductId = 6, Url = "https://localhost:7132/uploads/seedImages/logitech-mx-master-3-mouse.png" },

            // CLOTHING
            new() { Id = 7, ProductId = 7, Url = "https://localhost:7132/uploads/seedImages/basic-white-tshirt.png" },
            new() { Id = 8, ProductId = 8, Url = "https://localhost:7132/uploads/seedImages/slim-fit-jeans.png" },
            new() { Id = 9, ProductId = 9, Url = "https://localhost:7132/uploads/seedImages/hooded-sweatshirt.png" },
            new() { Id = 10, ProductId = 10, Url = "https://localhost:7132/uploads/seedImages/leather-jacket.png" },
            new() { Id = 11, ProductId = 11, Url = "https://localhost:7132/uploads/seedImages/running-shorts.png" },

            // HOME
            new() { Id = 12, ProductId = 12, Url = "https://localhost:7132/uploads/seedImages/wooden-dining-table.png" },
            new() { Id = 13, ProductId = 13, Url = "https://localhost:7132/uploads/seedImages/ergonomic-office-chair.png" },
            new() { Id = 14, ProductId = 14, Url = "https://localhost:7132/uploads/seedImages/led-table-lamp.png" },
            new() { Id = 15, ProductId = 15, Url = "https://localhost:7132/uploads/seedImages/blackout-curtain-set.png" },
            new() { Id = 16, ProductId = 16, Url = "https://localhost:7132/uploads/seedImages/silent-wall-clock.png" },

            // BOOKS
            new() { Id = 17, ProductId = 17, Url = "https://localhost:7132/uploads/seedImages/clean-code.png" },
            new() { Id = 18, ProductId = 18, Url = "https://localhost:7132/uploads/seedImages/design-patterns.png" },
            new() { Id = 19, ProductId = 19, Url = "https://localhost:7132/uploads/seedImages/1984-george-orwell.png" },
            new() { Id = 20, ProductId = 20, Url = "https://localhost:7132/uploads/seedImages/sapiens.png" },
            new() { Id = 21, ProductId = 21, Url = "https://localhost:7132/uploads/seedImages/the-pragmatic-programmer.png" },

            // HEALTH
            new() { Id = 22, ProductId = 22, Url = "https://localhost:7132/uploads/seedImages/vitamin-c-1000mg.png" },
            new() { Id = 23, ProductId = 23, Url = "https://localhost:7132/uploads/seedImages/protein-powder.png" },
            new() { Id = 24, ProductId = 24, Url = "https://localhost:7132/uploads/seedImages/digital-thermometer.png" },
            new() { Id = 25, ProductId = 25, Url = "https://localhost:7132/uploads/seedImages/hot-water-bottle.png" },
            new() { Id = 26, ProductId = 26, Url = "https://localhost:7132/uploads/seedImages/orthopedic-pillow.png" },

            // SPORTS
            new() { Id = 27, ProductId = 27, Url = "https://localhost:7132/uploads/seedImages/football-ball.png" },
            new() { Id = 28, ProductId = 28, Url = "https://localhost:7132/uploads/seedImages/yoga-mat.png" },
            new() { Id = 29, ProductId = 29, Url = "https://localhost:7132/uploads/seedImages/adjustable-dumbbell-set.png" },
            new() { Id = 30, ProductId = 30, Url = "https://localhost:7132/uploads/seedImages/jump-rope.png" },
            new() { Id = 31, ProductId = 31, Url = "https://localhost:7132/uploads/seedImages/resistance-band-set.png" },

            // TOYS
            new() { Id = 32, ProductId = 32, Url = "https://localhost:7132/uploads/seedImages/lego-city-set.png" },
            new() { Id = 33, ProductId = 33, Url = "https://localhost:7132/uploads/seedImages/remote-control-car.png" },
            new() { Id = 34, ProductId = 34, Url = "https://localhost:7132/uploads/seedImages/plush-teddy-bear.png" },
            new() { Id = 35, ProductId = 35, Url = "https://localhost:7132/uploads/seedImages/puzzle-1000-pieces.png" },
            new() { Id = 36, ProductId = 36, Url = "https://localhost:7132/uploads/seedImages/toy-train-set.png" },

            // AUTOMOTIVE
            new() { Id = 37, ProductId = 37, Url = "https://localhost:7132/uploads/seedImages/car-vacuum-cleaner.png" },
            new() { Id = 38, ProductId = 38, Url = "https://localhost:7132/uploads/seedImages/engine-oil-5w-30.png" },
            new() { Id = 39, ProductId = 39, Url = "https://localhost:7132/uploads/seedImages/car-battery-60ah.png" },
            new() { Id = 40, ProductId = 40, Url = "https://localhost:7132/uploads/seedImages/windshield-wipers.png" },
            new() { Id = 41, ProductId = 41, Url = "https://localhost:7132/uploads/seedImages/car-phone-holder.png" },

            // FURNITURE
            new() { Id = 42, ProductId = 42, Url = "https://localhost:7132/uploads/seedImages/3-seater-sofa.png" },
            new() { Id = 43, ProductId = 43, Url = "https://localhost:7132/uploads/seedImages/wardrobe-cabinet.png" },
            new() { Id = 44, ProductId = 44, Url = "https://localhost:7132/uploads/seedImages/tv-stand.png" },
            new() { Id = 45, ProductId = 45, Url = "https://localhost:7132/uploads/seedImages/nightstand.png" },
            new() { Id = 46, ProductId = 46, Url = "https://localhost:7132/uploads/seedImages/bookshelf.png" },

            // FOOD
            new() { Id = 47, ProductId = 47, Url = "https://localhost:7132/uploads/seedImages/organic-red-apples-1kg.png" },
            new() { Id = 48, ProductId = 48, Url = "https://localhost:7132/uploads/seedImages/natural-honey-500g.png" },
            new() { Id = 49, ProductId = 49, Url = "https://localhost:7132/uploads/seedImages/aged-white-cheese.png" },
            new() { Id = 50, ProductId = 50, Url = "https://localhost:7132/uploads/seedImages/extra-virgin-olive-oil.png" },
            new() { Id = 51, ProductId = 51, Url = "https://localhost:7132/uploads/seedImages/whole-wheat-bread.png" }
        };
    }
}
