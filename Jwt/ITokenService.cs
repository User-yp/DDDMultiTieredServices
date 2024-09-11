using System.Security.Claims;

namespace Jwt;

public interface ITokenService
{
    string BuildToken(IEnumerable<Claim> claims, JWTOptions jwt);
}
