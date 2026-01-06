using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Models.DTO.Seller
{
    public class SellerDetailDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }

        public List<SellerProductDto> Products { get; set; }
    }
}
