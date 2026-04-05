using qltc_ai.Models;

namespace qltc_ai.Service.Base
{
    public interface IUserService
    {
        Nguoidung CreateUser(int id, string email);
    }
}
