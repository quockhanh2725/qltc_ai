using Microsoft.AspNetCore.Mvc;
using qltc_ai.Service;

namespace qltc_ai.Controllers
{
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            var acc = _accountService.GetAccountAll();
            return Ok(acc);
            //return View();
        }
    }
}
