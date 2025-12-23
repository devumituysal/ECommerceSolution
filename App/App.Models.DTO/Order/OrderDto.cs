using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Models.DTO.Order
{
    public class OrderDto
    {
        public string OrderCode { get; set; }
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalPrice { get; set; }
        public int TotalProducts { get; set; }
        public int TotalQuantity { get; set; }
    }
}
