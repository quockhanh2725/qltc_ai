using qltc_ai.Models;

namespace qltc_ai.Service.Base
{
    public interface IAuthService
    {
        RegisterResult Register(string email);
        bool IsEmailExists(string email);
        void SaveOtp(string email, string password);
        bool VerifyOtp(string email, string otp);

    }
}
