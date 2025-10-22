using Core.DTOs;
using OnlineStore.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IProductBusiness
    {
        Task<PagedResult<ProductDTO>> GetAsync(ProductQuery filters);
        Task<ProductDTO?> GetByIdAsync(int id);
    }
}
