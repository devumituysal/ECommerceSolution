using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Models.DTO.Admin
{
    public class AdminNotificationDto
    {
        public int NewUsers { get; set; }          
        public int SellerRequests { get; set; }

        public int TotalCount => NewUsers + SellerRequests;
    }
}
