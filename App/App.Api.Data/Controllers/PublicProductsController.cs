using App.Data.Entities;
using App.Data.Repositories.Abstractions;
using App.Models.DTO.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

[ApiController]
[Route("api/products")]
public class PublicProductsController : ControllerBase
{
    private readonly IDataRepository _repo;

    public PublicProductsController(IDataRepository repo)
    {
        _repo = repo;
    }

    // GET api/products  (home listing)
    [HttpGet]
    public async Task<IActionResult> GetPublicProducts([FromQuery] int? categoryId , [FromQuery] string? q)
    {
        var query = _repo.GetAll<ProductEntity>()
               .Include(p => p.Images)
               .Include(p => p.Category)
               .Where(p => p.Enabled == true)
               .AsQueryable();

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(p => p.Name.Contains(q) || p.Details.Contains(q));

        var products = await query
            .Select(p => new ProductListItemDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Details = p.Details,
                Stock = p.StockAmount,
                CreatedAt = p.CreatedAt,
                CategoryName = p.Category.Name,
                ImageUrl = p.Images
                    .OrderBy(i => i.Id)
                    .Select(i => i.Url)
                    .FirstOrDefault(),
                SellerId = p.SellerId
            })
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return Ok(products);
    }

    // GET api/products/{id} (home detail)
    [HttpGet("{productId:int}")]
    public async Task<IActionResult> GetPublicById(int productId)
    {
        var product = await _repo.GetAll<ProductEntity>()
            .Include(p => p.Images)
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (product == null)
            return NotFound();

        var dto = new ProductDetailDto
        {
            Id = product.Id,
            Name = product.Name,
            Details = product.Details,
            Price = product.Price,
            StockAmount = product.StockAmount,
            CategoryId = product.CategoryId,
            CategoryName = product.Category.Name,
            Images = product.Images
                .Select(i => i.Url)
                .ToList()
        };

        return Ok(dto);
    }

    [HttpGet("latest")]
    public async Task<IActionResult> GetLatest([FromQuery] int take = 8)
    {
        take = Math.Clamp(take, 1, 50);

        var products = await _repo.GetAll<ProductEntity>()
            .Include(p => p.Images)
            .OrderByDescending(p => p.CreatedAt)
            .Take(take)
            .Select(p => new ProductListItemDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                ImageUrl = p.Images
                    .OrderBy(i => i.Id)
                    .Select(i => i.Url)
                    .FirstOrDefault()
            })
            .ToListAsync();

        return Ok(products);
    }

}