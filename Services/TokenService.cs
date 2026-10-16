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
        {
            // Read JWT settings
            var jwtKey = _config["Jwt:Key"];
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var expiryValue = _config["Jwt:ExpiryMinutes"];

            // Check required settings
            if (string.IsNullOrWhiteSpace(jwtKey) ||
                string.IsNullOrWhiteSpace(issuer) ||
                string.IsNullOrWhiteSpace(audience))
            {
                throw new InvalidOperationException(
                    "JWT configuration is missing. Check Jwt:Key, Jwt:Issuer and Jwt:Audience in appsettings.json."
                );
            }

            // Fix the Double.Parse null error
            if (!double.TryParse(
                    expiryValue,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out double expiryMinutes) ||
                expiryMinutes <= 0)
            {
                throw new InvalidOperationException(
                    "Jwt:ExpiryMinutes is missing or invalid. Set it to 60 in appsettings.json."
                );
            }

            // Create user claims
            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.UserId.ToString()
                ),
                new Claim(
                    ClaimTypes.Email,
                    user.Email ?? ""
                ),
                new Claim(
                    ClaimTypes.Role,
                    role
                ),
                new Claim(
                    "FirstName",
                    user.FirstName ?? ""
                )
            };

            // Create the security key
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            );

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            // Generate the JWT token
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddYears(100),
                signingCredentials: credentials
            );

            // Return the generated token
            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}