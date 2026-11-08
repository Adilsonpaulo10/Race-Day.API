
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace RaceDay.Tests
{
    public static class TestTokenHelper
    {
        // Test-only key. Never use this key in production.
        public const string TestSigningKey =
            "RaceDay-Tests-Only-Signing-Key-2026-Long-Enough!";

        public const string TestIssuer = "RaceDay.Tests";
        public const string TestAudience = "RaceDay.Tests.Client";

        public static string CreateToken(
            string userId,
            string email,
            string role)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(TestSigningKey));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(
                issuer: TestIssuer,
                audience: TestAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
