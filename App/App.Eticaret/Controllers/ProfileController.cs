using App.Data.Contexts;
using App.Eticaret.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Eticaret.Controllers
{
    public class ProfileController : Controller
    {
        private readonly AppDbContext _dbContext;

        public ProfileController(AppDbContext dbContext)
        {
            _dbContext=dbContext;
        }

        [HttpGet("/profile")]
        public async Task<IActionResult> Details()
        {
            var userId = "1"; // login olmuş kullanıcının id si yazılmalı.

            if (userId is null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var userViewModel = await _dbContext.Users
                .Where(u => u.Id == int.Parse(userId)) // bu kısımı kontrol et
                .Select(u => new ProfileDetailsViewModel
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                })
                .FirstOrDefaultAsync();

            if (userViewModel is null)
            {
                return RedirectToAction("Login", "Auth");
            }

            string? previousSuccessMessage = TempData["SuccessMessage"]?.ToString();

            if (previousSuccessMessage is not null)
            {
                ViewBag.SuccessMessage= previousSuccessMessage;
            }

            return View(userViewModel);
        }

        [HttpPost("/profile")]
        public async Task<IActionResult> Edit([FromForm] ProfileDetailsViewModel editMyProfileModel)
        {
            //if (User.Identity?.IsAuthenticated)
            //{
            //    return RedirectToAction("Login", "Auth");
            //}

            //var user = await GetCurrentUserAsync();

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == 1); // geçici olarak yazıldı...

            if (user is null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
            {
                return View(editMyProfileModel);
            }
            
         
            user.FirstName = editMyProfileModel.FirstName;
            user.LastName = editMyProfileModel.LastName;

            if (!string.IsNullOrWhiteSpace(editMyProfileModel.Password) && editMyProfileModel.Password != "******")
            {
                user.Password = editMyProfileModel.Password;
            }

            await _dbContext.SaveChangesAsync();    

            TempData["SuccessMessage"] = "Profiliniz başarıyla güncellendi.";

            return RedirectToAction(nameof(Details));
        }

        [HttpGet("/my-orders")]
        public async Task<IActionResult> MyOrders()
        {
            var userId = "1"; // login olmuş kullanıcının id si yazılmalı.

            if (userId is null)
            {
                return RedirectToAction("Login", "Auth");
            }

            List<OrderViewModel> orders = await _dbContext.Orders
                .Where(o => o.UserId == int.Parse(userId)) // bu kısımı kontrol et
                .Select(o => new OrderViewModel
                {
                    OrderCode = o.OrderCode,
                    Address = o.Address,
                    CreatedAt = o.CreatedAt,
                    TotalPrice = o.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity),
                    TotalProducts = o.OrderItems.Count,
                    TotalQuantity = o.OrderItems.Sum(oi => oi.Quantity),
                })
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return View(orders);
        }

        [HttpGet("/my-products")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> MyProducts()
        {
            var userId = "1"; // login olmuş kullanıcının id si yazılmalı.

            if (userId is null)
            {
                return RedirectToAction("Login", "Auth");
            }

            List<MyProductsViewModel> products = await _dbContext.Products
                .Where(p => p.SellerId == int.Parse(userId)) // bu kısımı kontrol et
                .Select(p => new MyProductsViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Description = p.Details,
                    Stock = p.StockAmount,
                    CreatedAt = p.CreatedAt,
                })
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(products);
        }

    }
}
