using qltc_ai.Models;

namespace qltc_ai.Service.Base
{
    public interface ICategoryService
    {
        List<Danhmuc> GetAllCategory();
        List<ChiTietDanhMuc> GetCategoriesByBudget(int budgetId);
        (bool success, decimal thieu) UpdateLimit(int accId, int idDetail, decimal newLimit);
        void Rating(ChiTietDanhMuc detail);

    }
}
