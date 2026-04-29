using qltc_ai.Models;
using qltc_ai.Repositories;

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

        public List<Danhmuc> GetAllCategory()
        {
            return _categoryRepo.GetAllCateogry();
        }

        public List<ChiTietDanhMuc> GetCategoriesByBudget(int budgetId)
        {
            return _categoryRepo.GetByBudget(budgetId);
        }

        public (bool success, decimal thieu) UpdateLimit(int accId, int idDetail, decimal newLimit)
        {
            var chiTiet = _categoryRepo.FindById(idDetail);
            if (chiTiet == null)
                return (false, 0);

            var budget = _budgetRepo.FindById(chiTiet.IdNganSach ?? 0);
            if (budget == null || budget.IdTaiKhoan != accId)
                return (false, 0);

            var allCategories = _categoryRepo.GetByBudget(chiTiet.IdNganSach ?? 0);

            decimal tongHienTai = allCategories.Sum(x => x.GioiHanTien ?? 0);
            decimal gioiHanCu = chiTiet.GioiHanTien ?? 0;
            decimal tongMoi = tongHienTai - gioiHanCu + newLimit;
            decimal tongNganSach = budget.TongTien ?? 0;
            decimal daTieu = chiTiet.TienDaTieu ?? 0;

            if (newLimit < daTieu)
                return (false, daTieu - newLimit);

            if (tongMoi > tongNganSach)
                return (false, tongMoi - tongNganSach);

            chiTiet.GioiHanTien = newLimit;
            Rating(chiTiet);

            _categoryRepo.UpdateCategory(chiTiet);
            _categoryRepo.Save();

            return (true, 0);
        }

        public void Rating(ChiTietDanhMuc detail)
        {
            if (detail.GioiHanTien == null || detail.GioiHanTien <= 0)
            {
                detail.DanhGia = "TrungBinh";
                return;
            }

            var percent = (detail.TienDaTieu ?? 0) / detail.GioiHanTien;

            if (percent <= 0.5m)
                detail.DanhGia = "Tot";
            else if (percent <= 0.8m)
                detail.DanhGia = "TrungBinh";
            else
                detail.DanhGia = "Xau";
        }
    }
}
