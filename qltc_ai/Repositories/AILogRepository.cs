using Microsoft.EntityFrameworkCore;
using qltc_ai.Models;
using qltc_ai.Models.AI;

namespace qltc_ai.Repositories
{
    public class AILogRepository : BaseRepository, IAILogRepository
    {
        public AILogRepository(qltcContext context) : base(context)
        {

        }

        public void SaveLog(int accountId, string question, string answer)
        {
            var log = new AiLog
            {
                IdTaiKhoan = accountId,
                CauHoi = question,
                TraLoi = answer,
                NgayTao = DateTime.Now
            };
            _context.AiLog.Add(log);
            _context.SaveChanges();
        }

        public List<AiLogDto> GetAll()
        {
            return _context.AiLog
                .Include(l => l.IdTaiKhoanNavigation)
                    .ThenInclude(tk => tk.Nguoidung)
                .OrderByDescending(l => l.NgayTao)
                .Select(l => new AiLogDto
                {
                    IdLog = l.IdLog,
                    IdTaiKhoan = l.IdTaiKhoan ?? 0,
                    TenNguoiDung = l.IdTaiKhoanNavigation != null
                                    ? l.IdTaiKhoanNavigation.Nguoidung != null
                                        ? l.IdTaiKhoanNavigation.Nguoidung.TenNguoiDung
                                        : l.IdTaiKhoanNavigation.Email
                                    : "—",
                    CauHoi = l.CauHoi,
                    TraLoi = l.TraLoi,
                    NgayTao = l.NgayTao
                })
                .ToList();
        }

        public void DeleteAll()
        {
            _context.AiLog.RemoveRange(_context.AiLog);
            _context.SaveChanges();
        }
    }
}
