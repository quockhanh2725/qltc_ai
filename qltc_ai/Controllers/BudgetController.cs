using Microsoft.AspNetCore.Mvc;
using qltc_ai.Service.Base;

namespace qltc_ai.Controllers
{
    [Route("budget")]
    [CheckLogin]
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

            var result = _tranService.AddTransaction(accountId.Value, 0, money, note, typeTran);

            if (result.Success)
                return Ok(new
                {
                    message = result.Message
                });

            return BadRequest(new
            {
                message = result.Message
            });
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
