using Microsoft.AspNetCore.Mvc;
using qltc_ai.Service.Base;

namespace qltc_ai.Controllers
{
    [Route("budget")]
    public class BudgetController : Controller
    {
        private readonly IBudgetService _budgetService;
        public BudgetController(IBudgetService budgetService)
        {
            _budgetService = budgetService;
        }

        [HttpGet("")]
        public IActionResult Index()
        {  
            return View();
        }
        [HttpPut("add")]
        public IActionResult AddMoney(decimal money)
        {
            int? accountId = HttpContext.Session.GetInt32("AccountId");

            if (accountId == null)
                return Unauthorized("Vui Long Dang Nhap");

            bool success = _budgetService.AddBudget(accountId.Value, money);

            if (success)
                return Ok("OK");

            return BadRequest("Lỗi");
        }
    }
}
