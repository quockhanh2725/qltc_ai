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

        public List<Taikhoan> GetAccountAll()
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
                MatKhau = _tk.MatKhau,
                RoleId = 2,
                IsActive = 1,
                NgayTao = DateTime.Now
            };
            _repo.addAccount(ac);
            _repo.Save();
            return ac;

        }

        public Taikhoan? Authenticate(string email, string password)
        {
            return _repo.GetByEmailAndPassword(email, password);
        }
    }
}
