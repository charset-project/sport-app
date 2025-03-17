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
public class UserController(IUserRepository userRepository) : ControllerBase
{   
    private readonly IUserRepository _userRepository = userRepository;
   

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
    [Authorize(Roles = "None")]
    public async Task<IActionResult> AddRole([FromBody] string role)
    {
        var PhoneNumber =  User.FindFirst(ClaimTypes.Name)?.Value;
        if (PhoneNumber is null) return BadRequest("PhoneNumber is null");
        return Ok(await _userRepository.AddRole(PhoneNumber, role));
    }

    [HttpPost("AccsessToken")]
    public async Task<IActionResult> AccsessTokenGenerator([FromBody] string refreshToken)
    {
        return Ok(await _userRepository.GenerateAccessToken(refreshToken));
    }

    
}
