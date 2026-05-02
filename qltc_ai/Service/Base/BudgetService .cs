using qltc_ai.Models;
using qltc_ai.Repositories;

namespace qltc_ai.Service.Base
{
    public class BudgetService : IBudgetService
    {
        private readonly IBudgetRepository _repo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly ICategoryService _categoryService;

        public BudgetService(IBudgetRepository repo, ICategoryRepository categoryRepo, ICategoryService categoryService)
        {
            _repo = repo;
            _categoryRepo = categoryRepo;
            _categoryService = categoryService;
        }

       

        public bool ResetMonth(int accId, int month, int year)
        {
            var exists = _repo.GetByMonth(accId, month, year);
            if (exists != null)
                return false;

            var prev = _repo.GetLatest(accId);

            decimal newTotal = 0;

            if (prev != null)
            {
                var prevCategories = _categoryRepo.GetByBudgetC(prev.IdNganSach);
                decimal totalSpent = prevCategories.Sum(x => x.TienDaTieu ?? 0);
                newTotal = Math.Max(0, (prev.TongTien ?? 0) - totalSpent);
            }

            var newBudget = new Ngansach
            {
                IdTaiKhoan = accId,
                Thang = new DateTime(year, month, 1),
                TongTien = newTotal
            };

            _repo.AddBudget(newBudget);
            _repo.Save();

          
            if (prev != null)
            {
                var oldCategories = _categoryRepo.GetByBudgetC(prev.IdNganSach);

                var newCategories = oldCategories.Select(x => new ChiTietDanhMuc
                {
                    IdDanhMuc = x.IdDanhMuc,
                    IdNganSach = newBudget.IdNganSach,
                    GioiHanTien = 0,
                    TienDaTieu = 0,
                    DanhGia = "Tot"
                }).ToList();

                _categoryRepo.AddRange(newCategories);
                _categoryRepo.Save();
            }

            return true;
        }

        public void AutoResetIfNeeded(int accId)
        {
            var now = DateTime.Now;
            var exists = _repo.GetByMonth(accId, now.Month, now.Year);
            if (exists != null)
                return;

            ResetMonth(accId, now.Month, now.Year);
        }

        public bool AutoAddNewAccount(int accId)
        {
            var now = DateTime.Now;
            var exists = _repo.GetByMonth(accId, now.Month, now.Year);
            if (exists != null)
                return false;

            var budget = new Ngansach
            {
                IdTaiKhoan = accId,
                Thang = new DateTime(now.Year, now.Month, 1),
                TongTien = 0
            };

            _repo.AddBudget(budget);
            _repo.Save();


            var allDanhMuc = _categoryRepo.GetAllCateogry();

            var details = allDanhMuc.Select(dm => new ChiTietDanhMuc
            {
                IdDanhMuc = dm.IdDanhMuc,
                IdNganSach = budget.IdNganSach,
                GioiHanTien = 0,
                TienDaTieu = 0,
                DanhGia = "Tot"
            }).ToList();

            _categoryRepo.AddRange(details);
            _categoryRepo.Save();

            return true;
        }

        public Ngansach? GetBudgetByMonth(int accId, int month, int year)
        {
            return _repo.GetByMonth(accId, month, year);
        }

        public Ngansach? GetLatestBudget(int accId)
        {
            return _repo.GetLatest(accId);
        }
    }
}
