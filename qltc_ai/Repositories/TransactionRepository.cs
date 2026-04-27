using Microsoft.EntityFrameworkCore;
using qltc_ai.Models;

namespace qltc_ai.Repositories
{
    public class TransactionRepository : BaseRepository, ITransactionRepository
    {
        public TransactionRepository(qltcContext context) : base(context)
        {
        }

        public void AddTransaction(Giaodich gd)
        {
            _context.Giaodich.Add(gd);
        }

        public void DeleteTransaction(Giaodich gd)
        {
            _context.Remove(gd);
        }

        public List<Giaodich> GetAll()
        {
            return _context.Giaodich
                .Include(g => g.IdTaiKhoanNavigation)
                .Include(g => g.IdChiTietNavigation)
                    .ThenInclude(c => c!.IdDanhMucNavigation)
                .ToList();
        }

        public List<Giaodich> GetByAccount(int accountId)
        {
            return _context.Giaodich
                .Where(g => g.IdTaiKhoan == accountId)
                .Include(g => g.IdChiTietNavigation)
                    .ThenInclude(c => c!.IdDanhMucNavigation)
                .OrderByDescending(g => g.NgayGiaoDich)
                .ToList();
        }

        public List<Giaodich> GetByCategory(int detailId)
        {
            return _context.Giaodich
                .Where(g => g.IdChiTiet == detailId)
                .OrderByDescending(g => g.NgayGiaoDich)
                .ToList();
        }

        public List<Giaodich> GetByAccountAndMonth(int accountId, int month, int year)
        {
            return _context.Giaodich
                .Where(g => g.IdTaiKhoan == accountId
                         && g.NgayGiaoDich.HasValue
                         && g.NgayGiaoDich.Value.Month == month
                         && g.NgayGiaoDich.Value.Year == year)
                .Include(g => g.IdChiTietNavigation)
                    .ThenInclude(c => c!.IdDanhMucNavigation)
                .OrderByDescending(g => g.NgayGiaoDich)
                .ToList();
        }

        public Giaodich? FindById(int id)
        {
            return _context.Giaodich
                .Include(g => g.IdChiTietNavigation)
                    .ThenInclude(c => c!.IdDanhMucNavigation)
                .FirstOrDefault(g => g.IdGiaoDich == id);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Update(Giaodich gd)
        {
            _context.Update(gd);
        }
    }
}
