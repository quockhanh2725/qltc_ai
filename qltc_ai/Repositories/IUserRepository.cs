using qltc_ai.Models;

namespace qltc_ai.Repositories
{
    public interface IUserRepository
    {
        void AddUser(Nguoidung ng);
        void Save();
        bool IsUsernameTaken(string username);
        Nguoidung? GetByAccountId(int accId);
        void UpdateUser(Nguoidung ng);
    }
}
