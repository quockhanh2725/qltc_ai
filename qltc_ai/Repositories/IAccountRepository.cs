using qltc_ai.Models;

namespace qltc_ai.Repositories
{
    public interface IAccountRepository
    {
        List<object> GetAll();
        void AddAccount(Taikhoan tk);
        void Save();
        Taikhoan? FindById(int id);
        bool IsEmailTaken(string email);
        Taikhoan? GetByEmailAndPassword(string email, string password);
        void DeleteAccount(Taikhoan tk);
    }
}
