using App.Data.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Data.Entities
{
    public class ProductEntity : BaseEntity
    {
        public int SellerId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public string Details { get; set; } = null!;
        public byte StockAmount { get; set; }
        public bool Enabled { get; set; } = true;

        

        public UserEntity Seller { get; set; } = null!;
        public CategoryEntity Category { get; set; } = null!;
       


        public ICollection<ProductImageEntity> Images { get; set; } = null!;
        public ICollection<ProductCommentEntity> Comments { get; set; } = null!;
    }
}
