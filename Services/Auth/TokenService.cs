using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AchievementsPlatform.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace AchievementsPlatform.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TokenService> _logger;

        public TokenService(IConfiguration configuration, ILogger<TokenService> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger;
        }

        public string GenerateToken(string steamId, string username, TimeSpan expiration)
        {
            if (string.IsNullOrEmpty(steamId))
                throw new ArgumentException("SteamId é obrigatório.", nameof(steamId));

            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("Username é obrigatório.", nameof(username));

            var secretKey = _configuration["Jwt:SecretKey"]
                ?? throw new InvalidOperationException("Jwt:SecretKey não configurado.");

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("steamId", steamId)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.Add(expiration),
                signingCredentials: creds
            );

            _logger.LogInformation($"Token gerado para o SteamId: {steamId}.");
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public string GetTokenId(string token)
        {
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            return jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value
                ?? throw new InvalidOperationException("Token ID (JTI) não encontrado.");
        }

        public TimeSpan GetTokenExpiration(string token)
        {
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            return jwtToken.ValidTo - DateTime.UtcNow;
        }
    }
}