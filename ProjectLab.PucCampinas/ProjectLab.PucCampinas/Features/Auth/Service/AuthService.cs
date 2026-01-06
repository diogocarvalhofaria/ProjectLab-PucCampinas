using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProjectLab.PucCampinas.Common.Services;
using ProjectLab.PucCampinas.Features.Auth.DTOs;
using ProjectLab.PucCampinas.Features.Users.Model;
using ProjectLab.PucCampinas.Infrastructure.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjectLab.PucCampinas.Features.Auth.Service
{
    public class AuthService : BaseService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext context, IConfiguration configuration, ICustomErrorHandler errorHandler) : base(errorHandler)
        {
            _context = context;
            _configuration = configuration;
        }

        private byte[] GetJwtKey()
        {
            var key = _configuration["JwtSettings:SecretKey"];
            if (string.IsNullOrEmpty(key))
                throw new Exception("Chave JWT não configurada no appsettings.json");

            return Encoding.ASCII.GetBytes(key);
        }

        public async Task<LoginResponse> Login(LoginRequest request)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Ra == request.Ra);

                if (user == null)
                    throw new Exception("Usuário ou senha inválidos.");

                if (user.PasswordHash != request.Password)
                    throw new Exception("Usuário ou senha inválidos.");

                var token = GenerateJwtToken(user);

                return new LoginResponse
                {
                    Id = user.Id,
                    Name = user.Name,
                    Ra = user.Ra,
                    Role = user.Role,
                    Token = token
                };
            }
            catch (Exception ex)
            {
                OnError(ex, 500);
                throw;
            }
        }

        public async Task SetPassword(string ra, string newPassword)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Ra == ra);

            if (user == null)
                throw new Exception("Usuário não encontrado.");

            user.PasswordHash = newPassword;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        private string GenerateJwtToken(User user)
        {
            var key = GetJwtKey();

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("Ra", user.Ra)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateSetupToken(User user)
        {
            var key = GetJwtKey();

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Ra", user.Ra),
                    new Claim("Type", "SetupPassword")
                }),
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string ValidateTokenAndGetRa(string token)
        {
            var key = GetJwtKey();
            var handler = new JwtSecurityTokenHandler();

            try
            {
                var validations = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };

                var claims = handler.ValidateToken(token, validations, out var tokenSecure);

                var type = claims.FindFirst("Type")?.Value;
                if (type != "SetupPassword") throw new Exception("Token inválido");

                return claims.FindFirst("Ra")?.Value ?? throw new Exception("Token inválido");
            }
            catch
            {
                throw new Exception("Link inválido ou expirado.");
            }
        }
    }
}