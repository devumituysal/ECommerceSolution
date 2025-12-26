using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Models.DTO.Product
{
    public class AddProductImagesRequestDto
    {
        public List<string> ImageUrls { get; set; } = new();
    }
}
