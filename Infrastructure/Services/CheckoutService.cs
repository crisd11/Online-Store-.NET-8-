using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly IConfiguration _config;
        private readonly OnlineStoreDbContext _db; // tu DbContext

        public CheckoutService(IConfiguration config, OnlineStoreDbContext db)
        {
            _config = config;
            _db = db;
            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
        }

        public async Task<string> CreateCheckoutSessionAsync(string userId)
        {
            // 1) Traer carrito del usuario
            var uid = Guid.Parse(userId);
            var cartItems = await _db.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.UserId == uid)
                .ToListAsync();

            if (cartItems.Count == 0)
                throw new ApplicationException("El carrito está vacío.");

            // 2) Mapear a LineItems de Stripe
            var lineItems = cartItems.Select(ci => new SessionLineItemOptions
            {
                Quantity = ci.Quantity,
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "usd", // o la que uses
                    UnitAmount = (long)(ci.Product.Price * 100m), // en centavos
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = ci.Product.Name,
                        Description = ci.Product.Description
                    }
                }
            }).ToList();

            var frontendBase = _config["Frontend:BaseUrl"]?.TrimEnd('/');

            var options = new SessionCreateOptions
            {
                Mode = "payment",
                LineItems = lineItems,
                SuccessUrl = $"{frontendBase}/checkout/success",
                CancelUrl = $"{frontendBase}/checkout/cancel"
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            return session.Url; // URL hosted de Stripe
        }
    }
}
