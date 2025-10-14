using Core.DTOs;
using OnlineStore.Core.DTOs;

namespace OnlineStore.Core.Interfaces
{
    public interface IProductService
    {
        Task<PagedResult<ProductDTO>> GetAsync(ProductQuery query);
        Task<ProductDTO?> GetByIdAsync(int id);

        // (Admin - opcional para más adelante)
        // Task<ProductDto> CreateAsync(ProductDto dto);
        // Task<ProductDto> UpdateAsync(Guid id, ProductDto dto);
        // Task DeleteAsync(Guid id);
    }
}
