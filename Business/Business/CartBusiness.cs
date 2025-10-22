using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Business
{
    public class CartBusiness : ICartBusiness
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartBusiness(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CartItem>> GetCartAsync(int userId)
        {
            return await _unitOfWork.CartRepository.GetCartAsync(userId);
        }

        public async Task AddToCartAsync(int userId, int productId, int quantity)
        {
            var product = await _unitOfWork.CartRepository.ExistsInCart(userId, productId);

            if (product != null)
            {
                product.Quantity += quantity;
            }
            else
            {
                var item = new CartItem
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = quantity
                };
                await _unitOfWork.CartRepository.Add(item);
            }
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateQuantityAsync(int userId, int productId, int quantity)
        {
            var product = await _unitOfWork.CartRepository.ExistsInCart(userId, productId);

            if (product != null)
            {
                product.Quantity = quantity;
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task RemoveFromCartAsync(int userId, int productId)
        {
            var cartItem = await _unitOfWork.CartRepository.ExistsInCart(userId, productId);

            if (cartItem != null)
            {
                await _unitOfWork.CartRepository.Delete(cartItem.Id);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync(int userId)
        {
            var items = _unitOfWork.CartRepository.GetAll().Where(c => c.UserId == userId);
            await _unitOfWork.CartRepository.DeleteAll(items);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
