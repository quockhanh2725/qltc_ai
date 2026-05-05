using Microsoft.AspNetCore.Mvc;
using qltc_ai.Service.Base;

namespace qltc_ai.Controllers
{
    [Route("budget")]
    //[CheckLogin]
    public class BudgetController : Controller
    {
        private readonly IBudgetService _budgetService;
        private readonly ITransactionService _tranService;
        public BudgetController(IBudgetService budgetService , ITransactionService tranService)
        {
            _budgetService = budgetService;
            _tranService = tranService;
        }

        
        [HttpPut("add")]
        public IActionResult AddMoney(decimal money , string note , string typeTran)
        {

            var accountId = HttpContext.Session.GetInt32("AccountId");

            bool success = _tranService.AddTransaction(accountId.Value, 0, money, note, typeTran);

            if (success)
                return Ok("OK");

            return BadRequest("Lỗi");
        }

        [HttpGet("current")]
        public IActionResult GetCurrentBudget()
        {
            var accId = HttpContext.Session.GetInt32("AccountId");
            var now = DateTime.Now;

            var budget = _budgetService.GetBudgetByMonth(accId.Value, now.Month, now.Year);

            return Ok(budget);
        }
    }
}
