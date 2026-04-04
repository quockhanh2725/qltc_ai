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
    }
}
