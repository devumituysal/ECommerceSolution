using App.Admin.Models.ViewModels;
using App.Data.Contexts;
using App.Data.Entities;
using App.Data.Repositories.Abstractions;
using App.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json;

namespace App.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }


        public async Task<IActionResult> List()
        {
           var result = await _userService.GetUsersAsync();

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = "Failed to retrieve users.";
                return View(new List<UserListItemViewModel>());
            }

            var users = result.Value
                .Select(u => new UserListItemViewModel
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Role = u.Role,
                    Enabled = u.Enabled,
                    IsBanned = u.IsBanned,
                    HasSellerRequest = u.HasSellerRequest
                })
                .ToList();

            return View(users);
        }

        [HttpPost]
        [Route("/user/{id:int}/enable")]
        public async Task<IActionResult> Enable(int id)
        {
            var result = await _userService.EnableAsync(id);

            if (!result.IsSuccess)
                TempData["ErrorMessage"] = "The user could not be activated.";
            else
                TempData["SuccessMessage"] = "User activated.";

            return RedirectToAction(nameof(List));
        }

        
        [HttpPost]
        [Route("/user/{id:int}/disable")]
        public async Task<IActionResult> Disable(int id)
        {
            var result = await _userService.DisableAsync(id);

            if (!result.IsSuccess)
                TempData["ErrorMessage"] = "The user could not be deactivated.";
            else
                TempData["SuccessMessage"] = "The user has been deactivated.";

            return RedirectToAction(nameof(List));
        }


        [Route("/user/{id:int}/approve")]
        [HttpGet]
        public async Task<IActionResult> Approve([FromRoute] int id)
        {
            var result = await _userService.ApproveAsync(id);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = "User verification failed.";
                return RedirectToAction(nameof(List));
            }

            TempData["SuccessMessage"] = "The user has been successfully approved.";
            return RedirectToAction(nameof(List));
        }

        [HttpPost]
        [Route("/user/{id:int}/revoke-seller")]
        public async Task<IActionResult> RevokeSeller([FromRoute] int id)
        {
            var result = await _userService.RevokeSellerAsync(id);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = "The user's reseller status could not be cancelled.";
                return RedirectToAction(nameof(List));
            }

            TempData["SuccessMessage"] = "The user's reseller status has been successfully cancelled.";
            return RedirectToAction(nameof(List));
        }

    }
}
