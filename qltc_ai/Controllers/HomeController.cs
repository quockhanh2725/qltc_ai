using Microsoft.AspNetCore.Mvc;
using qltc_ai.Models;
using System;

namespace qltc_ai.Controllers
{
    public class HomeController : Controller
    {
        private readonly qltcContext _context;

        public HomeController(qltcContext context)
        {
            _context = context;
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
    }
}
