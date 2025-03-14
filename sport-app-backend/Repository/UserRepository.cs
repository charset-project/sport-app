using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

    public async Task<AddRoleResponse> AddRole(User user, string role)
    {
        if(role.Equals("Coach")){
            user.TypeOfUser = TypeOfUser.Coach;
            user.Coach = new Coach();
            user.Coach.User=user;
            user.Coach.UserId=user.Id;
            await _context.Coaches.AddAsync(user.Coach);
            await _context.SaveChangesAsync();
            await _userManager.AddToRoleAsync(user, "Coach");

            return new AddRoleResponse(){Message="Coach added successfully",
               RefreshToken= user.RefreshToken,
                AccessToken = _tokenService.CreateToken(user),
                TypeOfUser = user.TypeOfUser};

        }else if(role.Equals("Athlete")){
            user.TypeOfUser = TypeOfUser.Athlete;
            user.Athlete = new Athlete();
            user.Athlete.User=user;
            user.Athlete.UserId=user.Id;
            _context.Athletes.Add(user.Athlete);
            await _context.SaveChangesAsync();
            await _userManager.AddToRoleAsync(user, "Athlete");

            return new AddRoleResponse(){
                Message="Athlete added successfully",
                RefreshToken= user.RefreshToken,
                AccessToken = _tokenService.CreateToken(user),
                TypeOfUser = user.TypeOfUser
            };
            }else{
                return new AddRoleResponse(){Message="Role not found"};
            }
    }

    public async Task<CheckCodeResponseDto> CheckCode(CheckCodeRequestDto checkCodeRequestDto)
    {
        var user = await _context.CodeVerifies.FirstOrDefaultAsync(x => x.PhoneNumber == checkCodeRequestDto.PhoneNumber);
    
        if (user != null)
        {
            _context.CodeVerifies.Remove(user);
            await _context.SaveChangesAsync();
            if (user.TimeCodeSend.AddMinutes(15) < DateTime.Now)
            {
                return new CheckCodeResponseDto()
                {
                    IsSuccess = false,
                    Message = "TimeOut"
                };
            }
            else
            {
                if (user.Code == checkCodeRequestDto.Code){
                    var userEntity = await _userManager.FindByNameAsync(checkCodeRequestDto.PhoneNumber);
                    if (userEntity != null)
                    {   

                        return new CheckCodeResponseDto()
                        {
                            IsSuccess = true,
                            Message = "CodeIsCorrect",
                            RefreshToken = _tokenService.CreateRefreshToken(userEntity),
                            AccessToken = _tokenService.CreateToken(userEntity),
                            TypeOfUser = userEntity.TypeOfUser
                        };
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
                            return new CheckCodeResponseDto()
                            {
                                IsSuccess = true,
                                Message = "CodeIsCorrect",
                                RefreshToken = newUser.RefreshToken,
                                AccessToken = _tokenService.CreateToken(newUser),
                                TypeOfUser = newUser.TypeOfUser
                            };
                        }
                    }

                    {
                        return new CheckCodeResponseDto()
                        {
                            IsSuccess = false,
                            Message = "CodeIsNotCorrect"
                        };
                    }
                }}

            return new CheckCodeResponseDto()
            {
                IsSuccess = false,
                Message = "Code Is Not Correct"
            };
        }

        return new CheckCodeResponseDto()
        {
            IsSuccess = false,
            Message = "User not found"
        };
    }


    public async Task<string> Login(string UserPhoneNumber)
    {
        var user = await _context.CodeVerifies.FirstOrDefaultAsync(x => x.PhoneNumber == UserPhoneNumber);
        if (user is null)
        {
            await _context.CodeVerifies.AddAsync(new CodeVerify()
            {
                PhoneNumber = UserPhoneNumber,
                Code = await _sendVerifyCode.SendCode(UserPhoneNumber),
                TimeCodeSend = DateTime.Now
            });
            await _context.SaveChangesAsync();


            return "CodeIsSuccessFullySend";
        }
        else
        {
            if (user.TimeCodeSend.AddMinutes(2) < DateTime.Now)
            {
                _context.CodeVerifies.Remove(user);
                await _context.SaveChangesAsync();
                await _context.CodeVerifies.AddAsync(new CodeVerify()
                {
                    PhoneNumber = UserPhoneNumber,
                    Code = await _sendVerifyCode.SendCode(UserPhoneNumber),
                    TimeCodeSend = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return "CodeIsSuccessFullySend";
            }
            else
            {
                return "you should wait 2 minutes";

            }
        }
    }

    

}
