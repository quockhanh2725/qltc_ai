using qltc_ai.Models;

namespace qltc_ai.Service.Base
{
    public interface IAccountService
    {
        List<Taikhoan> GetAccountAll();
        Taikhoan CreateAccount(Taikhoan tk);
        Taikhoan? GetAccountById(int id);
        Taikhoan? Authenticate(string email, string password);
        bool DeleteAccount(int id);
    }
}
