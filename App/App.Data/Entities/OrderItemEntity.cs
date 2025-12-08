using App.Data.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Data.Entities
{
    public class OrderItemEntity : BaseEntity
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public byte Quantity { get; set; }
        public decimal UnitPrice { get; set; }

       
        public OrderEntity Order { get; set; } = null!;
        public ProductEntity Product { get; set; } = null!;
    }
}
