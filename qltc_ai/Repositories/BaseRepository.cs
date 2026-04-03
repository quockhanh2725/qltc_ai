using Microsoft.AspNetCore.Mvc;
using qltc_ai.Models;

namespace qltc_ai.Repositories
{
    public class BaseRepository : Controller
    {
        protected readonly qltcContext _context;

        public BaseRepository(qltcContext context)
        {
            _context = context;
        }
    }
}
