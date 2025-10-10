using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class AuthResponse
    {
        public string Token { get; set; } = null!;
        public int ExpiresIn { get; set; }
        public string Role { get; set; } = null!;
    }
}
