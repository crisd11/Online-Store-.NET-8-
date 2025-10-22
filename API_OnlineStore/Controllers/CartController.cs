using Core.DTOs;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace API_OnlineStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartBusiness _cartBusiness;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartController(ICartBusiness cartBusiness, IHttpContextAccessor accessor)
        {
            _cartBusiness = cartBusiness;
            _httpContextAccessor = accessor;
        }

        private int GetUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User
                ?? throw new Exception("No HttpContext o usuario no autenticado.");

            var subClaim = user.FindFirst(ClaimTypes.NameIdentifier) ??
                            user.FindFirst(JwtRegisteredClaimNames.Sub) ??
                             user.FindFirst("sub");

            if (subClaim == null)
                throw new Exception("No se encontró el claim que identifica al usuario (sub/nameidentifier).");

            if (!int.TryParse(subClaim.Value, out var userId))
                throw new Exception($"El claim de Id no es un int válido: {subClaim.Value}");

            return userId;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetUserId();
            var cart = await _cartBusiness.GetCartAsync(userId);
            return Ok(cart);
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] CartDTO dto)
        {
            var userId = GetUserId();
            await _cartBusiness.AddToCartAsync(userId, dto.ProductId, dto.Quantity);
            return Ok();
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] CartDTO dto)
        {
            var userId = GetUserId();
            await _cartBusiness.UpdateQuantityAsync(userId, dto.ProductId, dto.Quantity);
            return Ok();
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> Remove(int productId)
        {
            var userId = GetUserId();
            await _cartBusiness.RemoveFromCartAsync(userId, productId);
            return Ok();
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> Clear()
        {
            var userId = GetUserId();
            await _cartBusiness.ClearCartAsync(userId);
            return Ok();
        }
    }

}
