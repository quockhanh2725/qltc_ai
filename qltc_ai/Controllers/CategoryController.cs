using Microsoft.AspNetCore.Mvc;
using qltc_ai.Service.Base;

namespace qltc_ai.Controllers
{
    [Route("category")]
    [CheckLogin]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _cateService;
        public CategoryController(ICategoryService cateService)
        {
            _cateService = cateService;
        }
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("all")]
        public IActionResult All()
        {
            var lis = _cateService.GetAllCategory();
            return Ok(lis);
        }
        [HttpGet("by-budget")]
        public IActionResult GetByBudget(int budgetId)
        {
            var list = _cateService.GetCategoriesByBudget(budgetId);
            return Ok(list);
        }
        [HttpPut("ulimit")]
        public IActionResult UpdateLimit(int idDetail, decimal newLimit)
        {
            var accId = HttpContext.Session.GetInt32("AccountId");

            var result = _cateService.UpdateLimit(accId.Value, idDetail, newLimit);

            if (result.success)
                return Ok(new { message = "cap nhat thanh cong" });

            return BadRequest(new
            {
                message = $"vuot ngan sach, thieu {result.thieu} tien"
            });
        }
    }
}
