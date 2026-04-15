using Microsoft.AspNetCore.Mvc;
using qltc_ai.Service.Base;

namespace qltc_ai.Controllers
{
    [Route("category")]
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

        [HttpPut("ulimit")]
        public IActionResult UpdateLimit(int idCate, decimal newLimit)
        {
            var accId = HttpContext.Session.GetInt32("AccountId");

            if (accId == null)
                return Unauthorized(new { message = "vui long dang nhap" });

            var result = _cateService.UpdateLimit(accId.Value, idCate, newLimit);

            if (result.success)
                return Ok(new { message = "cap nhat thanh cong" });

            return BadRequest(new
            {
                message = $"vuot ngan sach, thieu {result.thieu} tien"
            });
        }
    }
}
