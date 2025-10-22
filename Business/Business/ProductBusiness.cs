using Core.DTOs;
using Core.Interfaces;
using Core.Interfaces.Repositories;
using Microsoft.Extensions.Configuration;
using OnlineStore.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Business
{
    public class ProductBusiness : IProductBusiness
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductBusiness(IUnitOfWork unitOfWork, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
        }
        
        public async Task<PagedResult<ProductDTO>> GetAsync(ProductQuery filters)
        {
            var activeProductsQuery = await _unitOfWork.ProductRepository.ActiveProducts();

            if (!string.IsNullOrWhiteSpace(filters.Search))
            {
                var search = filters.Search.Trim().ToLower();
                activeProductsQuery = activeProductsQuery.Where(p =>
                    p.Name.ToLower().Contains(search) ||
                    (p.Description != null && p.Description.ToLower().Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(filters.Category))
            {
                var category = filters.Category.Trim().ToLower();
                activeProductsQuery = activeProductsQuery.Where(p => p.Category != null && p.Category.ToLower() == category);
            }

            if (filters.MinPrice.HasValue)
                activeProductsQuery = activeProductsQuery.Where(p => p.Price >= filters.MinPrice.Value);

            if (filters.MaxPrice.HasValue)
                activeProductsQuery = activeProductsQuery.Where(p => p.Price <= filters.MaxPrice.Value);

            var sortBy = (filters.SortBy ?? "createdAt").ToLower();
            var sortDir = (filters.SortDir ?? "desc").ToLower();

            activeProductsQuery = (sortBy, sortDir) switch
            {
                ("name", "asc") => activeProductsQuery.OrderBy(p => p.Name),
                ("name", "desc") => activeProductsQuery.OrderByDescending(p => p.Name),
                ("price", "asc") => activeProductsQuery.OrderBy(p => p.Price),
                ("price", "desc") => activeProductsQuery.OrderByDescending(p => p.Price),
                ("createdat", "asc") => activeProductsQuery.OrderBy(p => p.CreatedAt),
                _ => activeProductsQuery.OrderByDescending(p => p.CreatedAt)
            };

            var page = filters.Page <= 0 ? 1 : filters.Page;
            var pageSize = filters.PageSize <= 0 ? 12 : Math.Min(filters.PageSize, 100);

            var total = activeProductsQuery.Count();
            var items = activeProductsQuery
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
                .ToList();

            return new PagedResult<ProductDTO>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalItems = total
            };
        }

        public async Task<ProductDTO?> GetByIdAsync(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetById(id);

            return new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Category = product.Category,
                ImageUrl = product.ImageUrl
            };
        }
    }
}
