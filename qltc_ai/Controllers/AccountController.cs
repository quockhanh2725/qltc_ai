using Microsoft.AspNetCore.Mvc;
using qltc_ai.Models;
using qltc_ai.Models.Enum;
using qltc_ai.Service.Base;

namespace qltc_ai.Controllers
{
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IAuthService _authService;
        private readonly IBudgetService _budgetService;

        public AccountController(IAccountService accountService, IAuthService authService , IBudgetService budgetService)
        {
            _accountService = accountService;
            _authService = authService;
            _budgetService = budgetService;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost("send")]
        public IActionResult SendOtp(string email, string password)
        {
            try
            {
                if (_authService.IsEmailExists(email))
                    return BadRequest(new { message = "Email đã tồn tại" });

                _authService.SaveOtp(email, password);

                return Ok(new { message = "Thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("verify")]
        public IActionResult VerifyOtp(string email, string otp)
        {
            var ok = _authService.VerifyOtp(email, otp);

            if (!ok)
                return BadRequest(new { message = "OTP sai hoặc hết hạn" });

            return Ok(new { message = "otp dung" });
        }
        [HttpPost("register")]
        public IActionResult Register(string email)
        {
            
            var result = _authService.Register(email);

            switch (result.Status)
            {
                case RegisterStatus.NotSendOtp:
                    return BadRequest(new { message = "chua gui otp" });

                case RegisterStatus.NotVerified:
                    return BadRequest(new { message = "chua xac thuc otp" });

                case RegisterStatus.Success:
                    return Ok(result.Data);

                default:
                    return BadRequest(new { message = "loi" });
            }
        }
        [HttpPost("login")]
        public IActionResult Authenticate( string email, string password)
        {
            var acc = _accountService.Authenticate(email, password);

            if (acc == null)
            {
                return BadRequest(new { message = "Tài khoản hoặc mật khẩu không đúng" });
            }

            if (acc.IsActive == 0)
                return BadRequest(new { message = "Tài khoản của bạn đã bị khoá" });

            HttpContext.Session.SetInt32("AccountId", acc.IdTaiKhoan);

            _budgetService.AutoResetIfNeeded(acc.IdTaiKhoan);

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
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteAccount(int id)
        {
            var tb = _accountService.DeleteAccount(id);

            if (!tb)
                return BadRequest(new { message = "k co tk" });
            return Ok();
        }
    }
}
