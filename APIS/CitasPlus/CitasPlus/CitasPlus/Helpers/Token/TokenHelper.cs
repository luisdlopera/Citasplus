using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CitasPlus.Helpers.Token
{
    public class TokenHelper : ITokenHelper
    {
        private readonly string KeyToken;
        public TokenHelper(IConfiguration configuration)
        {
            KeyToken = configuration.GetSection("Jwt:Key").Value;
        }
        public string CreateToken(IEnumerable<Claim>? claims, TimeSpan expiration)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KeyToken));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims ?? Enumerable.Empty<Claim>(),
                expires: DateTime.UtcNow.Add(expiration),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
