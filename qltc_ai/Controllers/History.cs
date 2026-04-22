using Microsoft.AspNetCore.Mvc;

namespace qltc_ai.Controllers
{
    [Route("history")]
    public class History : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
