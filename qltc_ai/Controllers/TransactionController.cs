using Microsoft.AspNetCore.Mvc;
using qltc_ai.Service.Base;

namespace qltc_ai.Controllers
{
    [Route("transaction")]
    [CheckLogin]
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
        [HttpPost("add")]
        public IActionResult Add(int idCate, decimal money, string note)
        {
            var accId = HttpContext.Session.GetInt32("AccountId");

            var ok = _tranService.AddTransaction(accId.Value, idCate, money, note);

            if (ok)
                return Ok(new { message = "them thanh cong" });

            return BadRequest(new { message = "du lieu sai hoac khac thang" });
        }

        [HttpPut("update")]
        public IActionResult Update(int idTran, decimal newMoney, string newNote)
        {
            var accId = HttpContext.Session.GetInt32("AccountId");

            var ok = _tranService.UpdateTransaction(accId.Value, idTran, newMoney, newNote);

            if (ok)
                return Ok(new { message = "sua thanh cong" });

            return BadRequest(new { message = "du lieu sai" });
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
