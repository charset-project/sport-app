using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using sport_app_backend.Configuration;
using sport_app_backend.Models.DTO.Requests;
using sport_app_backend.Models.DTO.Responses;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using System.Security.Cryptography;
using sport_app_backend.Models.Account;

namespace sport_app_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtConfig _jwtConfig;

        public SecurityController(UserManager<IdentityUser> userManager, IOptionsMonitor<JwtConfig> optionsMonitor)
        {
            _userManager = userManager;
            _jwtConfig = optionsMonitor.CurrentValue;
        }


        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDTO user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByNameAsync(user.PhoneNumber);
                if (existingUser != null)
                {
                    //کد تایید واسش اسال بشه
                    //await SendVerificationCodeAsync(existingUser.PhoneNumber);
                    //return Ok(new { Message = "Verification code sent." });
                }
                var newUser = new IdentityUser()
                {
                    PhoneNumber = user.PhoneNumber,
                    UserName = user.PhoneNumber
                };
                var isCreated = await _userManager.CreateAsync(newUser);
                if (isCreated.Succeeded)
                {
                    //اینجا هم باید کد تایید ارسال بشه
                    //await SendVerificationCodeAsync(newUser.PhoneNumber);
                    //return Ok(new { Message = "Verification code sent." });
                }
                else
                {
                    return BadRequest(new RegistrationResponse()
                    {
                        Errors = isCreated.Errors.Select(x => x.Description).ToList(),
                        Success = false
                    });
                }
            }
            return BadRequest(new RegistrationResponse()
            {
                Errors = new List<string>()
                {
                    "Invalid payload"
                },
                Success = false
            });
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] UserRegistrationDTO user)
        {
            if(ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByNameAsync(user.PhoneNumber);
                if (existingUser == null)
                {
                    return BadRequest(new RegistrationResponse()
                    {
                        Errors = new List<string>()
                        {
                            "Invalid login request"
                        },
                        Success = false
                    });
                }
                var isCodeValid = await VerifyVerificationCodeAsync(existingUser.PhoneNumber, user.VerificationCode);
                if (!isCodeValid)
                {
                    return BadRequest(new AuthResult()
                    {
                        Success = false,
                        Errors = new List<string> { "The verification code is invalid." }
                    });
                }

                var isNewUser = !await _userManager.HasPasswordAsync(existingUser);

                var jwtToken = GenerateJwtToken(existingUser);
                var refreshToken = GenerateRefreshToken(existingUser);

                return Ok(new AuthResult()
                {
                    Success = true,
                    JwtToken = jwtToken,
                    RefreshToken = refreshToken,
                    IsNewUser = isNewUser
                });
            }
            return BadRequest(new RegistrationResponse()
            {
                Errors = new List<string>()
                {
                    "Invalid payload"
                },
                Success = false
            });
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var storedRefreshToken = GetRefreshTokenFromDatabase(request.RefreshToken);
            if (storedRefreshToken == null || storedRefreshToken.ExpiryDate < DateTime.UtcNow)
            {
                return BadRequest(new AuthResult()
                {
                    Success = false,
                    Errors = new List<string> { "Refresh Token is invalied." }
                });
            }

            var user = await _userManager.FindByIdAsync(storedRefreshToken.UserId);
            if (user == null)
            {
                return BadRequest(new AuthResult()
                {
                    Success = false,
                    Errors = new List<string> { "User Not Found." }
                });
            }

            var jwtToken = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken(user);

            // اینجا هم باید رفرش توکن قبلی را از دیتابیس حذف کینم و جدیده را اضافه کینم
            // من ننوشتمش بعد اضافه ش کنیم

            return Ok(new AuthResult()
            {
                Success = true,
                JwtToken = jwtToken,
                RefreshToken = newRefreshToken
            });
        }

        private string GenerateJwtToken(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(6),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);
            return jwtToken;
        }

        private string GenerateRefreshToken(IdentityUser user)
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                var refreshToken = Convert.ToBase64String(randomNumber);

                // ذخیره Refresh Token در دیتابیس
                //SaveRefreshToken(user.Id, refreshToken);
                // اینو کدشو باید اضافه کنیم

                return refreshToken;
            }
        }

        private async Task<bool> VerifyVerificationCodeAsync(string phone, string code)
        {
            var user = await _userManager.FindByNameAsync(phone);
            var claims = await _userManager.GetClaimsAsync(user);
            var savedCode = claims.FirstOrDefault(c => c.Type == "VerificationCode")?.Value;

            return savedCode == code;
        }

        private RefreshTokenModel GetRefreshTokenFromDatabase(string refreshToken)
        {
            // کد اینجا هم باید اضافه کنیم
            // اینو همینجوری نوشتم که ارور نده فعلا
            // ولی از دیتابیس پیدا کنه برگردونه
            return new RefreshTokenModel
            {
                UserId = "TestUserId",
                Token = refreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(7)
            };
        }
    }
}
