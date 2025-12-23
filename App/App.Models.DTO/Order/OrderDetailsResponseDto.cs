using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Models.DTO.Order
{
    public class OrderDetailsResponseDto
    {
        public string OrderCode { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public string Address { get; set; } = null!;
        public List<OrderItemDto> Items { get; set; } = new();

    }

}
