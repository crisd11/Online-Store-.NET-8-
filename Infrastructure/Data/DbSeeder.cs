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
                new Product { Name = "Sony Headphones", Price = 59.99m, Category = "Audio", ImageUrl = "https://sony.scene7.com/is/image/sonyglobalsolutions/Headphones_Product-finder_GH-H2?$productFinder$&fmt=png-alpha", Description = "Clear sound and integrated mic" },
                new Product { Name = "Wireless Mouse", Price = 24.90m, Category = "Peripheral", ImageUrl = "https://www.perozzi.com.ar/41253-large_default/logitech-mouse-gaming-g203-lightsync-negro.jpg", Description = "Precision and confort for long sessions" },
                new Product { Name = "Mechanical Keyboard", Price = 89.00m, Category = "Peripheral", ImageUrl = "https://media.wired.com/photos/65b0438c22aa647640de5c75/master/pass/Mechanical-Keyboard-Guide-Gear-GettyImages-1313504623.jpg", Description = "Cute colours in the dark" },
                new Product { Name = "Kindle Tablet", Price = 299.99m, Category = "Tablets", ImageUrl = "https://fastly.picsum.photos/id/367/4928/3264.jpg?hmac=H-2OwMlcYm0a--Jd2qaZkXgFZFRxYyGrkrYjupP8Sro", Description = "See the difference, Literally" },
                new Product { Name = "MacBook Air", Price = 1499.99m, Category = "Notebooks", ImageUrl = "https://fastly.picsum.photos/id/370/4928/3264.jpg?hmac=UGe0txSnG4hhV-fAoi7e3mTnvQFhYYNcPJJbYFePh5Q", Description = "Better processing than ever" },
                new Product { Name = "Nikon Camera", Price = 399.99m, Category = "Cameras", ImageUrl = "https://fastly.picsum.photos/id/250/4928/3264.jpg?hmac=4oIwzXlpK4KU3wySTnATICCa4H6xwbSGifrxv7GafWU", Description = "Be a professional with this top notch equipment" },
                new Product { Name = "Kindle Tablet", Price = 199.99m, Category = "Cameras", ImageUrl = "https://fastly.picsum.photos/id/454/4403/2476.jpg?hmac=pubXcBaPumNk0jElL63xrQYiSwQWA_DtS8uNNV8PmIE", Description = "The FX-3 Super is what your memories need!" },
                new Product { Name = "Lenovo LOQ Gen 9", Price = 1999.99m, Category = "Notebooks", ImageUrl = "https://p1-ofp.static.pub/medias/26050595576_LOQ15IRX9WHBKLTLunaGreyIMG_202309261024281753202414594.png", Description = "The notebook your everyday needs" },
                new Product { Name = "Keyboard", Price = 49.00m, Category = "Peripheral", ImageUrl = "https://ctl.net/cdn/shop/products/ctl-ctl-wireless-keyboard-for-chrome-os-works-with-chromebook-29142051258456_1000x.jpg?v=1647376204", Description = "Regular" },
                new Product { Name = "Samsung Galaxy Tab S11", Price = 599.99m, Category = "Tablets", ImageUrl = "https://tienda.personal.com.ar/images/720/webp/Samsung-Galaxy-Tab-s11_1759950390136459354.png", Description = "Incredible feeling" },
                new Product { Name = "Instax", Price = 599.99m, Category = "Cameras", ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTBG_94ja6vTbSX1bGE0xkKHt_fvmESNB4xeA&s", Description = "Get your memories instantly" },
                new Product { Name = "Retro Mouse", Price = 29.00m, Category = "Peripheral", ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcT-eVTgIvNvHwX4mnvUlEVAxLIiIrdgkQWgsw&s", Description = "Back to the 90s" },
                new Product { Name = "Technics Headphones", Price = 199.99m, Category = "Audio", ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcS3C7DsjIppqGbmOX9cqqOpe2aJdsxGBXLxIQ&s", Description = "Best quality in business!" }
            };

            db.Products.AddRange(demo);
            await db.SaveChangesAsync();
        }
    }
}

