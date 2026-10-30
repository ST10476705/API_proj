using API_proj.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API_proj.Services
{
    public class TokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string CreateToken(User user, string role)
            => CreateToken(user, new[] { role });

        public string CreateToken(User user, IEnumerable<string> roles)
        {
            var jwtKey = _config["Jwt:Key"];
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var expiryValue = _config["Jwt:ExpiryMinutes"];

            if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Length < 32)
                throw new InvalidOperationException(
                    "Jwt:Key is missing or too short. It must be at least 32 characters (256 bits).");

            if (string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience))
                throw new InvalidOperationException(
                    "JWT configuration missing: Jwt:Issuer or Jwt:Audience.");

            if (!double.TryParse(
                    expiryValue,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out double expiryMinutes) ||
                expiryMinutes <= 0)
            {
                throw new InvalidOperationException(
                    "Jwt:ExpiryMinutes is missing or invalid. Set it in appsettings.json (e.g. 60).");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email,          user.Email ?? ""),
                new Claim("FirstName",               user.FirstName ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // One ClaimTypes.Role per role
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),  // ✅ uses config
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}