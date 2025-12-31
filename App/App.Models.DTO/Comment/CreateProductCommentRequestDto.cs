using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Models.DTO.Comment
{
    public class CreateProductCommentRequestDto
    {
        [Required(ErrorMessage = "Comment text is required.")]
        public string Text { get; set; } = null!;

        [Required(ErrorMessage = "Please select a rating.")]
        [Range(1, 5, ErrorMessage = "Please select a rating.")]
        public byte StarCount { get; set; }
    }
}
