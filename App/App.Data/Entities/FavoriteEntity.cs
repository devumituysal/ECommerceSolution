using App.Data.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Data.Entities
{
    public class FavoriteEntity : BaseEntity
    {
        public int UserId { get; set; }
        public UserEntity User { get; set; }

        public int ProductId { get; set; }
        public ProductEntity Product { get; set; }

    }
}
