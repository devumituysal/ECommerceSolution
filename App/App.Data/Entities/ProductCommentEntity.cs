using App.Data.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Data.Entities
{
    public class ProductCommentEntity : BaseEntity
    {
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public string Text { get; set; } = null!;
        public byte StarCount { get; set; }
        public bool IsConfirmed { get; set; } = false;

       
        public ProductEntity Product { get; set; } = null!;
        public UserEntity User { get; set; } = null!;
    }
}
