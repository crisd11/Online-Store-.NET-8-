using Core.DTOs;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace API_OnlineStore.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IAuthService _auth;

        public UsersController(IAuthService auth) => _auth = auth;

        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var email = User.FindFirstValue(JwtRegisteredClaimNames.Email)
                        ?? User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var user = await _auth.GetByEmailAsync(email);
            if (user == null)
                return NotFound();

            var dto = new UserDTO { Id = user.Id, Name = user.Name, Email = user.Email, Role = user.Role };
            return Ok(dto);
        }
    }
}
