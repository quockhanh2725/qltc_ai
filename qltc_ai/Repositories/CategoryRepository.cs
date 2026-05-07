using Microsoft.EntityFrameworkCore;
using qltc_ai.Models;

namespace qltc_ai.Repositories
{
    public class CategoryRepository : BaseRepository, ICategoryRepository
    {
        public CategoryRepository(qltcContext context) : base(context)
        {
        }


        public List<Danhmuc> GetAllCateogry()
        {
            return _context.Danhmuc.ToList();
        }

        public Danhmuc? FindDanhMucById(int id)
        {
            return _context.Danhmuc.Find(id);
        }

        public List<ChiTietDanhMuc> GetByBudgetC(int budgetId)
        {
            return _context.ChiTietDanhMuc
                .Include(x => x.IdDanhMucNavigation)
                .Where(x => x.IdNganSach == budgetId && x.IdDanhMucNavigation.LoaiDanhMuc != "ThuNhap")
                .ToList();
        }

        public ChiTietDanhMuc? FindById(int id)
        {
            return _context.ChiTietDanhMuc
                .Include(x => x.IdDanhMucNavigation)
                .FirstOrDefault(x => x.IdChiTiet == id);
        }

        public void AddRange(List<ChiTietDanhMuc> list)
        {
            _context.ChiTietDanhMuc.AddRange(list);
        }

        public void UpdateCategory(ChiTietDanhMuc detail)
        {
            _context.ChiTietDanhMuc.Update(detail);
        }
        public void Add(ChiTietDanhMuc detail)
        {
            _context.ChiTietDanhMuc.Add(detail);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public ChiTietDanhMuc? GetByBudgetT(int budgetId)
        {
            return _context.ChiTietDanhMuc
                 .Include(x => x.IdDanhMucNavigation)
                 .FirstOrDefault(x => x.IdNganSach == budgetId && x.IdDanhMucNavigation.LoaiDanhMuc == "ThuNhap");
        }
    }
}
