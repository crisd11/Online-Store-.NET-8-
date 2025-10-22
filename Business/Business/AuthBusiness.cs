using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Core.Business
{
    public class AuthBusiness : IAuthBusiness
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly IEmailBusiness _email;

        public AuthBusiness(IUnitOfWork unitOfWork, IConfiguration config, IEmailBusiness email)
        {
            _unitOfWork = unitOfWork;
            _config = config;
            _email = email;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            var userDatabase = await _unitOfWork.UserRepository.GetByEmailAsync(request.Email);
            if (userDatabase != null)
                throw new ApplicationException("Email already in use");

            var passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(request.Password);
            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = passwordHash,
                Role = "User"
            };

            await _unitOfWork.UserRepository.Add(user);
            await _unitOfWork.SaveChangesAsync();
            return GenerateResponse(user);
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _unitOfWork.UserRepository.GetByEmailAsync(request.Email);
            if (user == null)
                throw new ApplicationException("The email provided is not registered as a User");

            var valid = BCrypt.Net.BCrypt.EnhancedVerify(request.Password, user.PasswordHash);
            if (!valid)
                throw new ApplicationException("Invalid Password");

            return GenerateResponse(user);
        }

        private AuthResponse GenerateResponse(User user)
        {
            var token = GenerateJwtToken(user);
            var expires = int.Parse(_config["Jwt:ExpiresMinutes"] ?? "60");

            return new AuthResponse { Token = token, ExpiresIn = expires, Role = user.Role };
        }

        private string GenerateJwtToken(User user)
        {
            var key = _config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
            var issuer = _config["Jwt:Issuer"] ?? "OnlineStoreApi";
            var audience = _config["Jwt:Audience"] ?? "OnlineStoreClients";
            var expiresMinutes = int.Parse(_config["Jwt:ExpiresMinutes"] ?? "60");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("role", user.Role),
                new Claim("name", user.Name)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task RecoverPasswordAsync(RecoverPasswordRequest request)
        {
            var user = await _unitOfWork.UserRepository.GetByEmailAsync(request.Email);
            if (user == null)
                throw new ApplicationException("El email no está registrado.");

            user.ResetToken = Guid.NewGuid().ToString("N");
            user.ResetTokenExpiration = DateTime.UtcNow.AddHours(1);
            await _unitOfWork.SaveChangesAsync();

            var baseUrl = _config["BaseUrl"]?.TrimEnd('/');
            var resetLink = $"{baseUrl}/reset-password?token={user.ResetToken}";

            var body = $@"
                        <p>Hello,</p>
                        <p>We received a request to reset your password.</p>
                        <p>Clicl in the link to define a new password (valid for 1 hour):</p>
                        <p><a href=""{resetLink}"">{resetLink}</a></p>
                        <p>If it wasn't you, ignore this email.</p>";

            await _email.SendAsync(user.Email, "Recover Password", body);
        }

        public async Task ResetPasswordAsync(string token, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(newPassword))
                throw new ApplicationException("Token or password invalid.");

            var user = _unitOfWork.UserRepository.GetAll().Where(u => u.ResetToken == token && u.ResetTokenExpiration > DateTime.UtcNow).SingleOrDefault();

            if (user == null)
                throw new ApplicationException("Token is invalid or has expired.");

            user.PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(newPassword);
            user.ResetToken = null;
            user.ResetTokenExpiration = null;

            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
