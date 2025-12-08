using App.Data.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Data.Entities
{
    public class OrderEntity : BaseEntity
    {
        public int UserId { get; set; }
        public string OrderCode { get; set; } = null!;
        public string Address { get; set; } = null!;


        public UserEntity User { get; set; } = null!;

        public ICollection<OrderItemEntity> OrderItems { get; set; } = null!;
    }
}
