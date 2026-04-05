using qltc_ai.Models;

namespace qltc_ai.Repositories
{
    public interface IAccountRepository
    {
        List<Taikhoan> GetAll();
        void addAccount(Taikhoan tk);
        void Save();
        Taikhoan? FindById(int id);
        bool IsEmailTaken(string email);
    }
}
