using qltc_ai.Models;
using qltc_ai.Repositories;
using System.Text;

namespace qltc_ai.Service.Base
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _cateRepo;
        private readonly IBudgetRepository _budgetRepo;
        public CategoryService(IBudgetRepository budgetRepo, ICategoryRepository cateRepo)
        {
            _cateRepo = cateRepo;
            _budgetRepo = budgetRepo;
        }
        public List<Danhmuc> GetAllCatrgory(int id)
        {
            return _cateRepo.GetByBudget(id).ToList();
        }

        public (bool success, decimal thieu) UpdateLimit(int accId, int idCate, decimal newLimit)
        {
            var category = _cateRepo.GetCategoryById(idCate);
            if (category == null)
                return (false, 0);

            var budget = _budgetRepo.FindById(category.IdNganSach);
            if (budget == null)
                return (false, 0);

            if (budget.IdTaiKhoan != accId)
                return (false, 0);

            var categories = _cateRepo.GetByBudget(category.IdNganSach);

            decimal tongHienTai = categories.Sum(x => x.GioiHanTien ?? 0);
            decimal gioiHanCu = category.GioiHanTien ?? 0;

            decimal tongMoi = tongHienTai - gioiHanCu + newLimit;
            decimal tongNganSach = budget.TongTien ?? 0;

            decimal daTieu = category.TienDaTieu ?? 0;

            if (newLimit < daTieu)
            {
                return (false, daTieu - newLimit);
            }

            if (tongMoi > tongNganSach)
            {
                decimal thieu = tongMoi - tongNganSach;
                return (false, thieu);
            }

            category.GioiHanTien = newLimit;
            _cateRepo.UpdateCategory(category);
            _cateRepo.Save();

            return (true, 0);
        }
    }
}
