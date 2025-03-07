using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using sport_app_backend.Dtos;
using sport_app_backend.Models.Account;

namespace sport_app_backend.Controller;
[Route("api/account")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<User> _athleteManager;
    public AccountController(UserManager<User> athleteManager)
    {
        _athleteManager = athleteManager;
    }

    [HttpGet("test")]
    public async Task<IActionResult> Test()
    {
        return Ok("test");
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        var user = new User
        {
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            BirthDate = registerDto.BirthDate,
            PhoneNumber = registerDto.PhoneNumber,
            UserName = registerDto.FirstName + registerDto.LastName,
            Gender = Gender.female,
            TypeOfUser = TypeOfUser.Athlete

        };
        var result = await _athleteManager.CreateAsync(user);
        if (!result.Succeeded) return BadRequest(result.Errors);
        var role = await _athleteManager.AddToRoleAsync(user, "Athlete");
        if (!role.Succeeded) return BadRequest(role.Errors);

        return Ok(user);
    }


}
