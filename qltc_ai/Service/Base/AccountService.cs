using qltc_ai.Models;
using qltc_ai.Repositories;

namespace qltc_ai.Service.Base
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _repo;

        public AccountService(IAccountRepository repo)
        {
            _repo = repo;
        }

        public List<object> GetAccountAll()
        {
            return _repo.GetAll();
        }

        public Taikhoan? GetAccountById(int id)
        {
            return _repo.FindById(id);
        }

        public Taikhoan CreateAccount(Taikhoan _tk)
        {
            
            var ac = new Taikhoan
            {
                Email = _tk.Email,
                MatKhau = HashPassword(_tk.MatKhau ?? ""),
                RoleId = 2,
                IsActive = 1,
                NgayTao = DateTime.Now
            };
            _repo.AddAccount(ac);
            _repo.Save();
            return ac;

        }

        public Taikhoan? Authenticate(string email, string password)
        {
            return _repo.GetByEmailAndPassword(email, password);
        }

        public bool DeleteAccount(int id)
        {
            var acc = _repo.FindById(id);
            if (acc ==  null)
                return false;

            _repo.DeleteAccount(acc);
            _repo.Save();
            return true;
        }

        public bool UpdateStatus(int id, int isActive)
        {
            var tk = _repo.FindById(id);
            if (tk == null) return false;
            tk.IsActive = (sbyte?)isActive;
            _repo.Save();
            return true;
        }
        private string HashPassword(string password)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToHexString(hash).ToLower();
        }
    }
}
