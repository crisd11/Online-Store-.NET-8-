using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class CartService : ICartService
    {
        private readonly OnlineStoreDbContext _db;

        public CartService(OnlineStoreDbContext db)
        {
            _db = db;
        }

        public async Task<List<CartItem>> GetCartAsync(Guid userId)
        {
            return await _db.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task AddToCartAsync(Guid userId, int productId, int quantity)
        {
            var existing = await _db.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

            if (existing != null)
            {
                existing.Quantity += quantity;
            }
            else
            {
                var item = new CartItem
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = quantity
                };
                await _db.CartItems.AddAsync(item);
            }
            await _db.SaveChangesAsync();
        }

        public async Task UpdateQuantityAsync(Guid userId, int productId, int quantity)
        {
            var item = await _db.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

            if (item != null)
            {
                item.Quantity = quantity;
                await _db.SaveChangesAsync();
            }
        }

        public async Task RemoveFromCartAsync(Guid userId, int productId)
        {
            var item = await _db.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

            if (item != null)
            {
                _db.CartItems.Remove(item);
                await _db.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync(Guid userId)
        {
            var items = _db.CartItems.Where(c => c.UserId == userId);
            _db.CartItems.RemoveRange(items);
            await _db.SaveChangesAsync();
        }
    }

}
