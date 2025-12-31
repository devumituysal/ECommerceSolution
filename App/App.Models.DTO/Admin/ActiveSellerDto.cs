using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Models.DTO.Admin
{
    public class ActiveSellerDto
    {
        public string SellerFullName { get; set; } = null!;
        public int TotalProduct { get; set; }
        public int TotalSales { get; set; }
        public decimal TotalEarning { get; set; }
    }
}
