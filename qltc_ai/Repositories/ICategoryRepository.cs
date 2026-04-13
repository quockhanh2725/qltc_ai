using qltc_ai.Models;

namespace qltc_ai.Repositories
{
    public interface ICategoryRepository
    {
        List<Danhmuc> GetByBudget(int budgetId);
        void AddRange(List<Danhmuc> list);
        void Save();
        Danhmuc? GetCategoryById(int id);
    }
}
