using qltc_ai.Models;

namespace qltc_ai.Repositories
{
    public interface ICategoryRepository
    {
        List<Danhmuc> GetAllCateogry();
        Danhmuc? FindDanhMucById(int id);

        List<ChiTietDanhMuc> GetByBudget(int budgetId);
        void Add(ChiTietDanhMuc detail);
        ChiTietDanhMuc? FindById(int id);
        void AddRange(List<ChiTietDanhMuc> list);
        void UpdateCategory(ChiTietDanhMuc detail);
        void Save();
    }
}