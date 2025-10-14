using Core.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Core.DTOs;
using OnlineStore.Core.Interfaces;

namespace API_OnlineStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _products;

        public ProductsController(IProductService products) => _products = products;

        /// <summary>
        /// Listado con búsqueda/filtrado/paginación/sort
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResult<ProductDTO>>> Get([FromQuery] ProductQuery query)
        {
            var res = await _products.GetAsync(query);
            return Ok(res);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductDTO>> GetById(int id)
        {
            var res = await _products.GetByIdAsync(id);
            return res is null ? NotFound() : Ok(res);
        }

        // (Admin-only para CRUD futuro)
        // [HttpPost, Authorize(Policy = "AdminOnly")]
        // [HttpPut("{id:guid}"), Authorize(Policy = "AdminOnly")]
        // [HttpDelete("{id:guid}"), Authorize(Policy = "AdminOnly")]
    }
}
