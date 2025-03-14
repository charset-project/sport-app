using System.Threading.Tasks;

namespace sport_app_backend.Interface;

public interface ISendVerifyCodeService
{
    public Task<string> SendCode(string PhoneNumber);
}
