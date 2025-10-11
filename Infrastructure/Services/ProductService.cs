using Core.DTOs;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Core.DTOs;
using OnlineStore.Core.Interfaces;

namespace OnlineStore.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly OnlineStoreDbContext _db;

        public ProductService(OnlineStoreDbContext db) => _db = db;

        public async Task<PagedResult<ProductDTO>> GetAsync(ProductQuery q)
        {
            var query = _db.Products.AsNoTracking().Where(p => p.IsActive);

            if (!string.IsNullOrWhiteSpace(q.Search))
            {
                var s = q.Search.Trim().ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(s) ||
                    (p.Description != null && p.Description.ToLower().Contains(s)));
            }

            if (!string.IsNullOrWhiteSpace(q.Category))
            {
                var c = q.Category.Trim().ToLower();
                query = query.Where(p => p.Category != null && p.Category.ToLower() == c);
            }

            if (q.MinPrice.HasValue)
                query = query.Where(p => p.Price >= q.MinPrice.Value);

            if (q.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= q.MaxPrice.Value);

            // Sorting
            var sortBy = (q.SortBy ?? "createdAt").ToLower();
            var sortDir = (q.SortDir ?? "desc").ToLower();

            query = (sortBy, sortDir) switch
            {
                ("name", "asc") => query.OrderBy(p => p.Name),
                ("name", "desc") => query.OrderByDescending(p => p.Name),
                ("price", "asc") => query.OrderBy(p => p.Price),
                ("price", "desc") => query.OrderByDescending(p => p.Price),
                ("createdat", "asc") => query.OrderBy(p => p.CreatedAt),
                _ => query.OrderByDescending(p => p.CreatedAt) // createdAt desc default
            };

            var page = q.Page <= 0 ? 1 : q.Page;
            var pageSize = q.PageSize <= 0 ? 12 : Math.Min(q.PageSize, 100);

            var total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Category = p.Category,
                    ImageUrl = p.ImageUrl
                })
                .ToListAsync();

            return new PagedResult<ProductDTO>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalItems = total
            };
        }

        public async Task<ProductDTO?> GetByIdAsync(Guid id)
        {
            return await _db.Products.AsNoTracking()
                .Where(p => p.Id == id && p.IsActive)
                .Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Category = p.Category,
                    ImageUrl = p.ImageUrl
                })
                .SingleOrDefaultAsync();
        }
    }
}
