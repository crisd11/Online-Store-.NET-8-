using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace API_OnlineStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckoutController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly OnlineStoreDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CheckoutController(IConfiguration config, OnlineStoreDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _config = config;
            _db = db;
            _httpContextAccessor = httpContextAccessor;
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
                throw new Exception($"El claim de Id no es un GUID válido: {subClaim.Value}");

            return userId;
        }

        [HttpPost("create-checkout-session")]
        public async Task<IActionResult> CreateCheckoutSession()
        {
            var userId = GetUserId();

            var cartItems = await _db.CartItems.Include(ci => ci.Product).Where(ci => ci.UserId == userId).ToListAsync();

            if (!cartItems.Any())
                return BadRequest("El carrito está vacío.");

            var baseUrl = _config["BaseUrl"];

            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];

            var lineItems = cartItems.Select(item => new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmountDecimal = item.Product.Price * 100,
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.Product.Name
                    }
                },
                Quantity = item.Quantity
            }).ToList();

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = $"{baseUrl}/checkout-success?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{baseUrl}/checkout-cancel",
                Metadata = new Dictionary<string, string>{{ "userId", userId.ToString() }}
            };

            var service = new SessionService();
            var session = service.Create(options);

            return Ok(new { url = session.Url });
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var endpointSecret = _config["Stripe:WebhookSecret"];

            Event stripeEvent;
            try
            {
                stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    endpointSecret
                );
            }
            catch (Exception ex)
            {
                return BadRequest($"Webhook error: {ex.Message}");
            }

            if (stripeEvent.Type == "checkout.session.completed")
            {
                var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
                Console.WriteLine($"✅ Sesión Stripe confirmada: {session?.Id}");
                if (session != null)
                {
                    await ProcessOrderFromStripeSession(session);
                }
            }

            return Ok();
        }

        private async Task ProcessOrderFromStripeSession(Session session)
        {
            if (!session.Metadata.TryGetValue("userId", out var userIdStr) || string.IsNullOrWhiteSpace(userIdStr))
            {
                Console.WriteLine("❌ No se encontró userId en metadata del stripe session.");
                return;
            }

            if (!int.TryParse(userIdStr, out var userId))
            {
                Console.WriteLine($"❌ El userId no es un int válido: {userIdStr}");
                return;
            }

            var cartItems = await _db.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                Console.WriteLine("⚠️ No hay ítems en el carrito para este usuario.");
                return;
            }

            var order = new Order
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                Total = cartItems.Sum(ci => ci.Product.Price * ci.Quantity),
                Items = cartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.Product.Price
                }).ToList()
            };

            _db.Orders.Add(order);
            _db.CartItems.RemoveRange(cartItems);

            await _db.SaveChangesAsync();
        }
    }

}

