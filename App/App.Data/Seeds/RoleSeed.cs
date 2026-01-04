using App.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Data.Seeds
{
    public static class RoleSeed
    {
        public static List<RoleEntity> Data => new()
        {
            new RoleEntity() { Id = 1, Name = "admin", CreatedAt = DateTime.UtcNow },
            new RoleEntity() { Id = 2, Name = "seller", CreatedAt = DateTime.UtcNow },
            new RoleEntity() { Id = 3, Name = "buyer", CreatedAt = DateTime.UtcNow }
        };
    
    }
    
}
