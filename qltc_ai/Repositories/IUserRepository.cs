using qltc_ai.Models;

namespace qltc_ai.Repositories
{
    public interface IUserRepository
    {
        void addUser(Nguoidung ng);
        void Save();
        bool IsUsernameTaken(string username);
        Nguoidung? GetByAccountId(int accId);
        void updateUser(Nguoidung ng);
    }
}
