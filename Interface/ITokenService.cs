using sport_app_backend.Models.Account;

namespace sport_app_backend.Interface;

public interface ITokenService
{
        string CreateToken(User user);
        string CreateRefreshToken(User user);

}
