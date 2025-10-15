using Core.Entities;

namespace Core.Interfaces
{
    public interface ICartService
    {
        Task<List<CartItem>> GetCartAsync(Guid userId);
        Task AddToCartAsync(Guid userId, int productId, int quantity);
        Task UpdateQuantityAsync(Guid userId, int productId, int quantity);
        Task RemoveFromCartAsync(Guid userId, int productId);
        Task ClearCartAsync(Guid userId);
    }

}
