using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Models.DTO.Comment
{
    public class CreateProductCommentRequestDto
    {
        public string Text { get; set; } = null!;
        public byte StarCount { get; set; }
    }
}
