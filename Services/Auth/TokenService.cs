using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AchievementsPlatform.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace AchievementsPlatform.Services
{
    public class TokenService : ITokenService
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;

        public TokenService(IConfiguration configuration)
        {
            // Lê as configurações do JWT do appsettings.json
            _secretKey = configuration["Jwt:SecretKey"]
                ?? throw new ArgumentNullException("Chave secreta do JWT não configurada.");
            _issuer = configuration["Jwt:Issuer"]
                ?? throw new ArgumentNullException("Issuer do JWT não configurado.");
            _audience = configuration["Jwt:Audience"]
                ?? throw new ArgumentNullException("Audience do JWT não configurada.");
        }

        public string GenerateToken(string steamId, string username)
        {
            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, username ?? "Unknown"),
        new Claim("steamId", steamId),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), 
        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(), ClaimValueTypes.Integer64) // Data de criação do token
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1), 
                SigningCredentials = signingCredentials,
                Issuer = _issuer,
                Audience = _audience
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }


        public ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = true, // Verifica se o token está expirado
                ClockSkew = TimeSpan.Zero // Ignora qualquer diferença de relógio
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                return principal;
            }
            catch (Exception ex)
            {
                throw new SecurityTokenException("Token inválido ou expirado.", ex);
            }
        }
    }
}