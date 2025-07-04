using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using sport_app_backend.Data;
using sport_app_backend.Interface;
using sport_app_backend.Models;
using sport_app_backend.Models.Account;

namespace sport_app_backend.Services;

public class TokenService: ITokenService
{
    
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;
        private readonly ApplicationDbContext _context;
        private readonly DbSet<User> _userManager;
        public TokenService(IConfiguration config,ApplicationDbContext dbContext)
        {    
            _context = dbContext;
            _userManager = dbContext.Users;
            _config = config;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Signingkey"]));
        }

    public async Task<string> CreateRefreshToken(User user)
    {
        
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        user.RefreshToken = Convert.ToBase64String(randomNumber);
        // user.RefreshTokeNExpire= DateTime.Now.AddDays(90);
        await _context.SaveChangesAsync();
        return user.RefreshToken;
    }


    public string CreateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.PhoneNumber),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var userRoles = user.TypeOfUser switch
        {
            TypeOfUser.COACH => new[] { "Coach" },
            TypeOfUser.ATHLETE => new[] { "Athlete" },
            TypeOfUser.NONE => new[] { "None" },
            TypeOfUser.ADMIN => throw new NotImplementedException(),
            _ => throw new NotImplementedException(),
        };
        
        
        claims = claims.Concat(userRoles.Select(role => new Claim(ClaimTypes.Role, role))).ToArray();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature),
            Issuer = _config["JWT:Issuer"],
            Audience = _config["JWT:Audience"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    


}
