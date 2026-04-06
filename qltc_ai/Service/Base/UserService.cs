using qltc_ai.Models;
using qltc_ai.Repositories;

namespace qltc_ai.Service.Base
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;

        public UserService(IUserRepository repo)
        {
            _repo = repo;
        }

        public static string RandomNumber(int length = 4)
        {
            var random = new Random();
            return random.Next((int)Math.Pow(10, length - 1), (int)Math.Pow(10, length)).ToString();
        }

        public Nguoidung CreateUser(int id, string email)
        {
            string username;
            do
            {
                username = email.Split('@')[0] + "#" + RandomNumber();
            }
            while (_repo.IsUsernameTaken(username));

            var user = new Nguoidung
            {
                IdTaiKhoan = id,
                TenNguoiDung = username
            };

            _repo.addUser(user);
            _repo.Save();
            return user;
        }

        public bool UpdateUsername(int accountId, string newUsername)
        {
            if (newUsername == null || newUsername == "")
                return false;

            var user = _repo.GetByAccountId(accountId);

            if (user == null)
                return false;

            
            bool exist = _repo.IsUsernameTaken(newUsername) && user.TenNguoiDung != newUsername;

            if (exist)
                return false;

            user.TenNguoiDung = newUsername;

            _repo.updateUser(user);
            _repo.Save();

            return true;
        }
    }
}
