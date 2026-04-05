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
    }
}
