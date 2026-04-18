using Microsoft.AspNetCore.Mvc;
using qltc_ai.Models;
using qltc_ai.Service.Base;
using System;

namespace qltc_ai.Controllers
{
    [Route("")]
    [CheckLogin]
    public class HomeController : Controller
    {
        private readonly qltcContext _context;
        private readonly IUserService _userService;
        public HomeController(qltcContext context , IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public IActionResult Index()
        {
            bool isConnected = false;

            try
            {
                isConnected = _context.Database.CanConnect();
            }
            catch
            {
                isConnected = false;
            }

            return View(isConnected);
        }
        [HttpPut("updateinfo")]
        public IActionResult UpdateUsername(string newUsername)
        {
            int? accountId = HttpContext.Session.GetInt32("AccountId");

            bool success = _userService.UpdateUsername(accountId.Value, newUsername);

            if (success)
                return Ok("doi ten thanh cong");

            return BadRequest("ten ton tai hoac khong hop le");
        }
    }
}
