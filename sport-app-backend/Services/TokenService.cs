using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using sport_app_backend.Data;
using sport_app_backend.Interface;
using sport_app_backend.Models.Account;

namespace sport_app_backend.Services;

public class TokenService: ITokenService
{
    
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;
        private readonly ApplicationDbContext _context;
        public TokenService(IConfiguration config,ApplicationDbContext dbContext)
        {    
            _context = dbContext;
            _config = config;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Signinkey"]));
        }

    public string CreateRefreshToken(User user)
    {
        
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        user.RefreshToken = Convert.ToBase64String(randomNumber);
        user.RefreshTokeNExpire= DateTime.Now.AddDays(90);
        _context.SaveChanges();
        return user.RefreshToken;
    }


    public string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, user.PhoneNumber)
            };
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(30),
                SigningCredentials = creds,
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"]
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    


}
