using Microsoft.AspNetCore.Mvc;
using qltc_ai.Models;
using qltc_ai.Service.Base;

namespace qltc_ai.Controllers
{
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IAuthService _authService;

        public AccountController(IAccountService accountService, IAuthService authService)
        {
            _accountService = accountService;
            _authService = authService;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            var acc = _accountService.GetAccountAll();
            return Ok(acc);
            //return View();
        }
        [HttpPost("register")]
        public IActionResult Register([FromBody] Taikhoan _tk)
        {
            try
            {
                var result = _authService.Register(_tk);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("login")]
        public IActionResult Authenticate( string email, string password)
        {
            var acc = _accountService.Authenticate(email, password);

            if (acc == null)
            {
                return BadRequest(new { message = "sai" });
            }

            if (acc.IsActive == 0)
                return BadRequest(new { message = "tai khoan da bi khoa" });

            HttpContext.Session.SetInt32("AccountId", acc.IdTaiKhoan);

            switch (acc.RoleId)
            {
                case 2:
                    return Ok(new { message = "user" });
                case 1:
                    return Ok(new { message = "admin" });
                default:
                    return BadRequest(new { message = "k hop le" });
            }
        }
    }
}
