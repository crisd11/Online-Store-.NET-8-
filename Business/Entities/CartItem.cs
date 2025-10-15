using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class CartItem
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public User User { get; set; } = default!;
        public Product Product { get; set; } = default!;
    }
}
