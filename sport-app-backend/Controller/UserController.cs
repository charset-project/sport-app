using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using sport_app_backend.Dtos;
using sport_app_backend.Interface;
using sport_app_backend.Models.Account;

namespace sport_app_backend.Controller;
[Route("api/[controller]")]
[ApiController]
public class UserController(IUserRepository userRepository, UserManager<User> userManager) : ControllerBase
{   
    private readonly IUserRepository _userRepository = userRepository;
    private readonly UserManager<User> _userManager = userManager;

    [HttpPost("CheckCode")]
    public async Task<CheckCodeResponseDto> CheckCode([FromBody] CheckCodeRequestDto checkCodeRequestDto)
    {
        return await _userRepository.CheckCode(checkCodeRequestDto);
    }

    ///Send code
    [HttpPost("SendCode")]
    public async Task<string> SendCode([FromBody]string UserPhoneNumber)
    {
        return await _userRepository.Login(UserPhoneNumber);
    }

    [HttpPut("add role")]
    [Authorize]
    public  IActionResult AddRole([FromBody] string role){
    var PhoneNumber =  User.FindFirst(ClaimTypes.Name)?.Value;
    if(PhoneNumber is null) return BadRequest("PhoneNumber is null");
    var user = _userManager.FindByNameAsync(PhoneNumber).Result;
    if(user is null) return BadRequest("user is null");
    _userManager.AddToRoleAsync(user, role).Wait();
    return Ok();
    }

    
}
