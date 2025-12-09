using App.Data.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Data.Entities
{
    public class UserEntity : BaseEntity
    {
        public string Email { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? ResetPasswordToken { get; set; }
        public int RoleId { get; set; }
        public bool Enabled { get; set; } = true;
        public bool HasSellerRequest { get; set; } = false;

        
        public RoleEntity Role { get; set; } = null!;

    }
}
