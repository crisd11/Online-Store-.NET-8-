using Core.Entities;
using Core.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class CartRepository : BaseRepository<CartItem>, ICartRepository
    {
        public CartRepository(OnlineStoreDbContext dbContext) : base(dbContext)
        {
            
        }

        public async Task<List<CartItem>> GetCartAsync(int userId)
        {
            return await _entities.Where(c => c.UserId == userId).Include(c => c.Product).ToListAsync();
        }

        public async Task<CartItem> ExistsInCart(int userId, int productId) 
        {
            return await _entities.FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);
        }

        public async Task DeleteAll(IEnumerable<CartItem> items)
        {
            _entities.RemoveRange(items);
        }
    }
}
