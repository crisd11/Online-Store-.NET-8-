using Core.Entities;
using Infrastructure.Data;

using Core.Entities;

namespace OnlineStore.Infrastructure.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(OnlineStoreDbContext db)
        {
            if (db.Products.Any()) return;

            var demo = new[]
            {
                new Product { Name = "Auriculares Aycron A10", Price = 59.99m, Category = "Audio", ImageUrl = "https://picsum.photos/seed/a10/400/300", Description = "Sonido nítido y micrófono integrado." },
                new Product { Name = "Mouse Inalámbrico Aycron M5", Price = 24.90m, Category = "Periféricos", ImageUrl = "https://picsum.photos/seed/m5/400/300", Description = "Precisión y confort para largas horas." },
                new Product { Name = "Teclado Mecánico Aycron K60", Price = 89.00m, Category = "Periféricos", ImageUrl = "https://picsum.photos/seed/k60/400/300", Description = "Switches táctiles y retroiluminación." },
                new Product { Name = "Monitor 27\" Aycron V27", Price = 299.99m, Category = "Monitores", ImageUrl = "https://picsum.photos/seed/v27/400/300", Description = "144Hz, 1ms, panel IPS." },
                new Product { Name = "Webcam HD Aycron C1", Price = 39.99m, Category = "Video", ImageUrl = "https://picsum.photos/seed/c1/400/300", Description = "1080p con auto-focus." }
            };

            db.Products.AddRange(demo);
            await db.SaveChangesAsync();
        }
    }
}

