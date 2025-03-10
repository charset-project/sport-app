using System.Threading.Tasks;

namespace sport_app_backend.Interface;

public interface ISendVerifyCodeService
{
    public string SendCode(string PhoneNumber);
}
