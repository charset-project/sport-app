using sport_app_backend.Dtos;
using sport_app_backend.Models.Account;

namespace sport_app_backend.Interface;

public interface IUserRepository
{
    public Task<string> Login(string UserPhoneNumber);
    public Task<CheckCodeResponseDto> CheckCode(CheckCodeRequestDto checkCodeRequestDto);
    public Task<AddRoleResponse> AddRole(string phoneNumber, string role);
   // public Task<string> AddRole(string role);
}
