using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Models.DTO.Admin
{
    public class OrderListDto
    {
        public string OrderNumber { get; set; } = null!;
        public string UserFullName { get; set; } = null!;
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
