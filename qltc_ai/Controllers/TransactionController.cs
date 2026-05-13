using Microsoft.AspNetCore.Mvc;
using qltc_ai.Service.Base;

namespace qltc_ai.Controllers
{
    [Route("transaction")]
    //[CheckLogin]
    public class TransactionController : Controller
    {
        private readonly ITransactionService _tranService;
        public TransactionController(ITransactionService tranService)
        {
            _tranService = tranService;
        }
        [HttpGet("")]
        public IActionResult Index()
        {  
            return View();
        }
        [HttpGet("list")]
        public IActionResult List()
        {
            var now = DateTime.Now;
            var accId = HttpContext.Session.GetInt32("AccountId");

            var lis = _tranService.GetByAccountAndMonth(accId.Value , now.Month , now.Year);

            return Ok(lis);
        }
        [HttpPost("add")]
        public IActionResult Add(int idDetail, decimal money, string note , string typeTran)
        {
            var accId = HttpContext.Session.GetInt32("AccountId");

            var result = _tranService.AddTransaction(accId.Value, idDetail, money, note , typeTran);

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

        [HttpPut("update")]
        public IActionResult Update(int idTran, decimal newMoney, string newNote)
        {
            var accId = HttpContext.Session.GetInt32("AccountId");

            var ok = _tranService.UpdateTransaction(accId.Value, idTran, newMoney, newNote);

            if (ok)
                return Ok(new { message = "sua thanh cong" });

            return BadRequest(new { message = "Số tiền vượt quá giới hạn" });
        }

        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {

            var ok = _tranService.DeleteTransaction(id);

            if (ok)
                return Ok(new { message = "xoa thanh cong" });

            return BadRequest(new { message = "khong tim thay giao dich" });
        }
    }
}
