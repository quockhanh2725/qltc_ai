using qltc_ai.Models;
using qltc_ai.Repositories;

namespace qltc_ai.Service.Base
{
    public class AuthService : IAuthService
    {
        private readonly IAccountService _accountService;
        private readonly IUserService _userService;
        private readonly IAccountRepository _repo;

        public AuthService(IAccountService accountService, IUserService userService , IAccountRepository repo)
        {
            _accountService = accountService;
            _userService = userService;
            _repo = repo;
        }

        public Taikhoan Register(Taikhoan tk)
        {
            if(_repo.IsEmailTaken(tk.Email))
                return null;
            var acc = _accountService.CreateAccount(tk);

            var use = _userService.CreateUser(acc.IdTaiKhoan, tk.Email);
            
            return acc;
        }
        
    }
}
