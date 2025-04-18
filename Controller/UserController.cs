using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sport_app_backend.Dtos;
using sport_app_backend.Interface;
using sport_app_backend.Models;

namespace sport_app_backend.Controller;
[Route("api/[controller]")]
[ApiController]
public class UserController(IUserRepository userRepository) : ControllerBase

{
    [HttpPost("CheckCode")]
    public async Task<ApiResponse> CheckCode([FromBody] CheckCodeRequestDto checkCodeRequestDto)
    {
        return await userRepository.CheckCode(checkCodeRequestDto);
    }

    ///Send code
    [HttpPost("SendCode")]
    public async Task<ApiResponse> SendCode([FromBody]string userPhoneNumber)
    {
        return await userRepository.Login(userPhoneNumber);
    }

    [HttpPut("addRoleGender")]
    [Authorize(Roles = "None")]
    public async Task<IActionResult> AddRoleGender([FromBody] RoleGenderDto roleGenderDto)
    {
        var phoneNumber =  User.FindFirst(ClaimTypes.Name)?.Value;
        if (phoneNumber is null) return BadRequest("PhoneNumber is null");
        return Ok(await userRepository.AddRoleGender(phoneNumber, roleGenderDto));
    }

    [HttpPost("AccessToken")]
    public async Task<IActionResult> AccessToken([FromBody] string refreshToken)
    {
        return Ok(await userRepository.GenerateAccessToken(refreshToken));
    }
    
    //add username
    [HttpPut("add_username")]
    [Authorize(Roles = "Athlete,Coach")]
    public async Task<IActionResult> AddUsername([FromBody] string username)
    {
        var phoneNumber =  User.FindFirst(ClaimTypes.Name)?.Value;
        if (phoneNumber is null) return BadRequest("PhoneNumber is null");
        var result = await userRepository.AddUsername(phoneNumber, username);
        if (!result.Action) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("edit_user_profile")]
    [Authorize(Roles = "Athlete,Coach")]
    public async Task<IActionResult> EditUserProfile([FromBody] EditUserProfileDto userProfileDto)
    {
        var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
        if (phoneNumber is null) return BadRequest("PhoneNumber is null");
        var result = await userRepository.EditUserProfile(phoneNumber, userProfileDto);
        if (!result.Action) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("get_user_profile_for_edit")]
    [Authorize(Roles = "Athlete,Coach")]
    public async Task<IActionResult> GetUserProfileForEdit()
    {
        var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
        if (phoneNumber is null) return BadRequest("PhoneNumber is null");
        var result = await userRepository.GetUserProfileForEdit(phoneNumber);
        if (!result.Action) return NotFound(result);
        return Ok(result);
    }
    [HttpPost("report_app")]
    [Authorize(Roles = "Athlete,Coach")]
    public async Task<IActionResult> ReportApp([FromBody] ReportAppDto reportAppDto)
    {
        var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
        if (phoneNumber is null) return BadRequest("PhoneNumber is null");
        var result = await userRepository.ReportApp(phoneNumber, reportAppDto);
        if (!result.Action) return BadRequest(result);
        return Ok(result);
    }
    
    //logout
    [HttpDelete("logout")]
    [Authorize(Roles = "Athlete,Coach")]
    public async Task<IActionResult> Logout()
    {
        var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
        if (phoneNumber is null) return BadRequest("PhoneNumber is null");
        var result = await userRepository.Logout(phoneNumber);
        if (!result.Action) return NotFound(result);
        return Ok(result);
    }
    
    [HttpPost("upload")]
    [Authorize(Roles = "Athlete,Coach")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
        if (phoneNumber is null) return BadRequest("PhoneNumber is null");
        
        var result = await userRepository.SaveImageAsync(phoneNumber,file);

        if (!result.Action) return NotFound(result);
        return Ok(result);
    }
    



}
