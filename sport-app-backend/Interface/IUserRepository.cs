using sport_app_backend.Dtos;

namespace sport_app_backend.Interface;

public interface IUserRepository
{
    public Task<string> Login(string UserPhoneNumber);
    public Task<CheckCodeResponseDto> CheckCode(CheckCodeRequestDto checkCodeRequestDto);
   // public Task<string> AddRole(string role);
}
