using qltc_ai.Models;
using qltc_ai.Repositories;
using System.Text;

namespace qltc_ai.Service.Base
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepo;
        private readonly IBudgetRepository _budgetRepo;
        public CategoryService(IBudgetRepository budgetRepo, ICategoryRepository cateRepo)
        {
            _categoryRepo = cateRepo;
            _budgetRepo = budgetRepo;
        }
        public List<Danhmuc> GetAllCatrgory(int id)
        {
            return _categoryRepo.GetByBudget(id).ToList();
        }

        public (bool success, decimal thieu) UpdateLimit(int accId, int idCate, decimal newLimit)
        {
            var category = _categoryRepo.FindById(idCate);
            if (category == null)
                return (false, 0);

            var budget = _budgetRepo.FindById(category.IdNganSach);
            if (budget == null)
                return (false, 0);

            if (budget.IdTaiKhoan != accId)
                return (false, 0);

            var categories = _categoryRepo.GetByBudget(category.IdNganSach);

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
            Rating(category);
            _categoryRepo.UpdateCategory(category);
            _categoryRepo.Save();

            return (true, 0);
        }

        public void Rating(Danhmuc cate)
        {
            if (cate.GioiHanTien <= 0)
            {
                cate.DanhGia = "khong hop le";
                return;
            }

            var percent = (cate.TienDaTieu ?? 0) / cate.GioiHanTien;

            if (percent <= 0.5m)
                cate.DanhGia = "Tot";
            else if (percent <= 0.8m)
                cate.DanhGia = "TrungBinh";
            else
                cate.DanhGia = "Xau";
        }
    }
}
