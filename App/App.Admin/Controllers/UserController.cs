using App.Admin.Models.ViewModels;
using App.Data.Contexts;
using App.Data.Entities;
using App.Data.Repositories.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json;

namespace App.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    public class UserController : BaseController
    {
        public UserController(HttpClient httpClient) : base(httpClient) { }
        public async Task<IActionResult> List()
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Login", "Auth");
            }

            SetJwtHeader();

            var response = await _httpClient.GetAsync("https://localhost:5001/api/users/list");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Kullanıcılar alınamadı.";
                return View(new List<UserListItemViewModel>());
            }

            var users = await response.Content.ReadFromJsonAsync<List<UserListItemViewModel>>();

            users ??= new List<UserListItemViewModel>();

            return View(users);
        }


        [Route("/users/{id:int}/approve")]
        [HttpGet]
        public async Task<IActionResult> Approve([FromRoute] int id)
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Login", "Auth");
            }

            SetJwtHeader();

            var response = await _httpClient.PostAsync($"https://localhost:5001/api/users/{id}/approve", null);

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Kullanıcı onaylanamadı.";
                return RedirectToAction(nameof(List));
            }

            TempData["SuccessMessage"] = "Kullanıcı başarıyla onaylandı.";
            return RedirectToAction(nameof(List));
        }

    }
}
