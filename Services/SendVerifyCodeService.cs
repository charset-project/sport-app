using sport_app_backend.Interface;

namespace sport_app_backend.Services;

public class SendVerifyCodeService : ISendVerifyCodeService
{
   public async Task<string> SendCode(string PhoneNumber)
    {
        await Task.Delay(100);
        return "12345";
    }

}
