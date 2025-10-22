using Core.Entities;

namespace Core.Interfaces.Repositories
{
    public interface ICartRepository : IBaseRepository<CartItem>
    {
        Task<List<CartItem>> GetCartAsync(int userId);
        Task<CartItem> ExistsInCart(int userId, int productId);
        Task DeleteAll(IEnumerable<CartItem> items);
    }
}
