using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService(IConfiguration config) : ITokenService
{
    public string CreateToken(AppUser user)
    {
        var tokenKey = config["TokenKey"] ?? throw new Exception("Cannot get token key.");

        if (tokenKey.Length < 64) throw new Exception("token key length needs to be >= 64 characters");

        // signing key
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

        // claims
        var claims = new List<Claim>()
        {
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.NameIdentifier, user.Id)
        };

        // credentials
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = cred,
            Expires = DateTime.Now.AddDays(7)
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
