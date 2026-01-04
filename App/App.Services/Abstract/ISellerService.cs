using App.Models.DTO.Seller;
using Ardalis.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Abstract
{
    public interface ISellerService
    {
        Task<Result<SellerDetailDto>> GetSellerDetailAsync(int sellerId);
    }
}
