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
        private readonly ICartService _cartService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartController(ICartService cartService, IHttpContextAccessor accessor)
        {
            _cartService = cartService;
            _httpContextAccessor = accessor;
        }

        private Guid GetUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User
                ?? throw new Exception("No HttpContext o usuario no autenticado.");

            var subClaim = user.FindFirst(ClaimTypes.NameIdentifier) ??
                            user.FindFirst(JwtRegisteredClaimNames.Sub) ??
                             user.FindFirst("sub");

            if (subClaim == null)
                throw new Exception("No se encontró el claim que identifica al usuario (sub/nameidentifier).");

            if (!Guid.TryParse(subClaim.Value, out var userId))
                throw new Exception($"El claim de Id no es un GUID válido: {subClaim.Value}");

            return userId;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetUserId();
            var cart = await _cartService.GetCartAsync(userId);
            return Ok(cart);
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] CartDTO dto)
        {
            var userId = GetUserId();
            await _cartService.AddToCartAsync(userId, dto.ProductId, dto.Quantity);
            return Ok();
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] CartDTO dto)
        {
            var userId = GetUserId();
            await _cartService.UpdateQuantityAsync(userId, dto.ProductId, dto.Quantity);
            return Ok();
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> Remove(int productId)
        {
            var userId = GetUserId();
            await _cartService.RemoveFromCartAsync(userId, productId);
            return Ok();
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> Clear()
        {
            var userId = GetUserId();
            await _cartService.ClearCartAsync(userId);
            return Ok();
        }
    }

}
