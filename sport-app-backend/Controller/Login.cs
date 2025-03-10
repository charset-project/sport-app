using Microsoft.AspNetCore.Mvc;
using sport_app_backend.Dtos;
using sport_app_backend.Interface;

namespace sport_app_backend.Controller;
[Route("api/[controller]")]
[ApiController]
public class Login:ControllerBase
{   
    private readonly IUserRepository _userRepository;
    public Login(IUserRepository userRepository){
        _userRepository=userRepository;
    }

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

    
}
