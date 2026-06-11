using qltc_ai.Models;

namespace qltc_ai.Service.Base
{
    public interface IAccountService
    {
        List<object> GetAccountAll();
        Taikhoan CreateAccount(Taikhoan tk);
        Taikhoan? GetAccountById(int id);
        Taikhoan? Authenticate(string email, string password);
        bool DeleteAccount(int id);
        public bool UpdateStatus(int id, int isActive);
    }
}
