using Core.Entities;
using Infrastructure.Data;

namespace OnlineStore.Infrastructure.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(OnlineStoreDbContext db)
        {
            if (db.Products.Any())
                return;

            var demo = new[]
            {
                new Product { Name = "Sony Headphones", Price = 59.99m, Category = "Audio", ImageUrl = "https://sony.scene7.com/is/image/sonyglobalsolutions/Headphones_Product-finder_GH-H2?$productFinder$&fmt=png-alpha", Description = "Clear sound and integrated mic." },
                new Product { Name = "Wireless Mouse", Price = 24.90m, Category = "Peripheral", ImageUrl = "https://www.perozzi.com.ar/41253-large_default/logitech-mouse-gaming-g203-lightsync-negro.jpg", Description = "Precision and confort for long sessions." },
                new Product { Name = "Mechanical Keyboard", Price = 89.00m, Category = "Peripheral", ImageUrl = "https://media.wired.com/photos/65b0438c22aa647640de5c75/master/pass/Mechanical-Keyboard-Guide-Gear-GettyImages-1313504623.jpg", Description = "Cute colours in the dark." },
                new Product { Name = "Kindle Tablet", Price = 299.99m, Category = "Tablets", ImageUrl = "https://fastly.picsum.photos/id/367/4928/3264.jpg?hmac=H-2OwMlcYm0a--Jd2qaZkXgFZFRxYyGrkrYjupP8Sro", Description = "See the difference, Literally." },
                new Product { Name = "MacBook Air", Price = 1499.99m, Category = "Notebooks", ImageUrl = "https://fastly.picsum.photos/id/370/4928/3264.jpg?hmac=UGe0txSnG4hhV-fAoi7e3mTnvQFhYYNcPJJbYFePh5Q", Description = "Better processing than ever" },
            };

            db.Products.AddRange(demo);
            await db.SaveChangesAsync();
        }
    }
}

