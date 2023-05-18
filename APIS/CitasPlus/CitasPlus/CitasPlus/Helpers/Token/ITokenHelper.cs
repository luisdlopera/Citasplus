using System.Security.Claims;

namespace CitasPlus.Helpers.Token
{
    public interface ITokenHelper
    {
        string CreateToken(IEnumerable<Claim>? claims, TimeSpan expiration);
    }
}
