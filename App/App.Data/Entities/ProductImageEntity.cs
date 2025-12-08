using App.Data.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Data.Entities
{
    public class ProductImageEntity : BaseEntity
    {
        public int ProductId { get; set; }
        public string Url { get; set; } = null!;

        
        public ProductEntity Product { get; set; } = null!;
    }
}
