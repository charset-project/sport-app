using Microsoft.AspNetCore.Identity;
using sport_app_backend.Data;
using sport_app_backend.Dtos;
using sport_app_backend.Interface;
using sport_app_backend.Models.Account;
using sport_app_backend.Models.Login_Sinup;

namespace sport_app_backend.Repository;

public class UserRepository : IUserRepository
{
    private readonly UserManager<User> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly ISendVerifyCodeService _sendVerifyCode;
    private readonly ITokenService _tokenService;

    public UserRepository(ApplicationDbContext dbContext, ITokenService tokenService, UserManager<User> userManager, ISendVerifyCodeService sendVerifyCode)
    {
        _context = dbContext;
        _userManager = userManager;
        _sendVerifyCode = sendVerifyCode;
        _tokenService = tokenService;

    }

    public Task<CheckCodeResponseDto> CheckCode(CheckCodeRequestDto checkCodeRequestDto)
    {
        var user = _context.CodeVerifies.FirstOrDefault(x => x.PhoneNumber == checkCodeRequestDto.PhoneNumber);
    
        if (user != null)
        {
            _context.CodeVerifies.Remove(user);
            _context.SaveChanges();
            if (user.TimeCodeSend.AddMinutes(15) < DateTime.Now)
            {
                return Task.FromResult(new CheckCodeResponseDto()
                {
                    IsSuccess = false,
                    Message = "TimeOut"
                });
            }
            else
            {
                if (user.Code == checkCodeRequestDto.Code){
                    var userEntity = _userManager.FindByNameAsync(checkCodeRequestDto.PhoneNumber).Result;
                    if (userEntity != null)
                    {   

                        return Task.FromResult(new CheckCodeResponseDto()
                        {
                            IsSuccess = true,
                            Message = "CodeIsCorrect",
                            RefreshToken = _tokenService.CreateRefreshToken(userEntity),
                            AccessToken = _tokenService.CreateToken(userEntity),
                            TypeOfUser = userEntity.TypeOfUser
                        });
                    }
                    else
                    {   // create new user
                        var newUser = new User()
                        {
                            PhoneNumber = checkCodeRequestDto.PhoneNumber,
                            TypeOfUser = TypeOfUser.None,
                            UserName = checkCodeRequestDto.PhoneNumber,
                        };
                        _tokenService.CreateRefreshToken(newUser);
                        var result = _userManager.CreateAsync(newUser).Result;
                        var addRole = _userManager.AddToRoleAsync(newUser,  "none" ).Result;
                        if (result.Succeeded)
                        {
                            return Task.FromResult(new CheckCodeResponseDto()
                            {
                                IsSuccess = true,
                                Message = "CodeIsCorrect",
                                RefreshToken = newUser.RefreshToken,
                                AccessToken = _tokenService.CreateToken(newUser),
                                TypeOfUser = newUser.TypeOfUser
                            });
                        }
                    }

                    {
                        return Task.FromResult(new CheckCodeResponseDto()
                        {
                            IsSuccess = false,
                            Message = "CodeIsNotCorrect"
                        });
                    }
                }}

            return Task.FromResult(new CheckCodeResponseDto()
            {
                IsSuccess = false,
                Message = "Code Is Not Correct"
            });
        }

        return Task.FromResult(new CheckCodeResponseDto()
        {
            IsSuccess = false,
            Message = "User not found"
        });
    }


    public Task<string> Login(string UserPhoneNumber)
    {
        var user = _context.CodeVerifies.FirstOrDefault(x => x.PhoneNumber == UserPhoneNumber);
        if(user is null){
        _context.CodeVerifies.Add(new CodeVerify()
        {
            PhoneNumber = UserPhoneNumber,
            Code = _sendVerifyCode.SendCode(UserPhoneNumber),
            TimeCodeSend = DateTime.Now
        }
        );
        _context.SaveChanges();


        return Task.FromResult("CodeIsSuccessFullySend");}
        else{
            if(user.TimeCodeSend.AddMinutes(2) < DateTime.Now){
                _context.CodeVerifies.Remove(user);
                _context.SaveChanges();
                _context.CodeVerifies.Add(new CodeVerify()
                {
                    PhoneNumber = UserPhoneNumber,
                    Code = _sendVerifyCode.SendCode(UserPhoneNumber),
                    TimeCodeSend = DateTime.Now
                }
                );
                _context.SaveChanges();
                return Task.FromResult("CodeIsSuccessFullySend");
            }
            else{
                return Task.FromResult("you should wait 2 minutes");
                
            }
        }
    }

    

}
