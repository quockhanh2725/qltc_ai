using qltc_ai.Models;

namespace qltc_ai.Repositories
{
    public class CategoryRepository : BaseRepository, ICategoryRepository
    {
        public CategoryRepository(qltcContext context) : base(context)
        {
        }

        public void AddRange(List<Danhmuc> list)
        {
            _context.Danhmuc.AddRange(list);
        }

        public List<Danhmuc> GetByBudget(int budgetId)
        {
            return _context.Danhmuc.Where(x => x.IdNganSach == budgetId).ToList();
        }

        public Danhmuc? FindById(int id)
        {
            return _context.Danhmuc.Find(id);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void UpdateCategory(Danhmuc cate)
        {
            _context.Danhmuc.Update(cate);
        }
    }
}
