using Core.DTOs;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Core.DTOs;

namespace API_OnlineStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductBusiness _productsBusiness;

        public ProductsController(IProductBusiness productsBusiness)
        {
            _productsBusiness = productsBusiness;
        }

        /// <summary>
        /// Listado con búsqueda/filtrado/paginación/sort
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResult<ProductDTO>>> Get([FromQuery] ProductQuery query)
        {
            var res = await _productsBusiness.GetAsync(query);
            return Ok(res);
        }

        /// <summary>
        /// Search by Id
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductDTO>> GetById(int id)
        {
            var res = await _productsBusiness.GetByIdAsync(id);
            return res is null ? NotFound() : Ok(res);
        }
    }
}
