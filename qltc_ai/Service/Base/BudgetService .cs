using qltc_ai.Models;
using qltc_ai.Repositories;

namespace qltc_ai.Service.Base
{
    public class BudgetService : IBudgetService
    {
        private readonly IBudgetRepository _repo;
        private readonly ICategoryRepository _categoryRepo;
        public BudgetService(IBudgetRepository repo,ICategoryRepository categoryRepo)
        {
            _repo = repo;
            _categoryRepo = categoryRepo;
        }

        public bool AddBudget(int accId, decimal money)
        {
            if (money <= 0)
                return false;

            var now = DateTime.Now;
            var ns = _repo.GetByMonth(accId, now.Month, now.Year);

            AutoResetIfNeeded(accId);
            if (ns == null)
                return false;
            ns.TongTien = ns.TongTien + money;

            _repo.UpdateBudget(ns);
            _repo.Save();

            return true;
        }

        public bool ResetMonth(int accId, int month, int year)
        {
            var exists = _repo.GetByMonth(accId, month, year);
            if (exists != null)
                return false;

            var prev = _repo.GetLatest(accId);

            decimal? newTotal = 0;

            if (prev != null)
            {
                var categories = _categoryRepo.GetByBudget(prev.IdNganSach);

                decimal totalSpent = categories.Sum(x => x.TienDaTieu ?? 0);

                newTotal = prev.TongTien - totalSpent;
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
                var oldCategories = _categoryRepo.GetByBudget(prev.IdNganSach);

                var newCategories = oldCategories.Select(x => new Danhmuc
                {
                    TenDanhMuc = x.TenDanhMuc,
                    Mau = x.Mau,
                    GioiHanTien = 0,
                    TienDaTieu = 0,
                    DanhGia = "Tot",
                    IdNganSach = newBudget.IdNganSach
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

            var defaultCategories = new List<Danhmuc>
            {
                new Danhmuc { TenDanhMuc = "Ăn uống", GioiHanTien = 0, TienDaTieu = 0, Mau = "#FF6384",DanhGia = "Tot", IdNganSach = budget.IdNganSach },
                new Danhmuc { TenDanhMuc = "Đi lại", GioiHanTien = 0, TienDaTieu = 0, Mau = "#36A2EB",DanhGia = "Tot", IdNganSach = budget.IdNganSach },
                new Danhmuc { TenDanhMuc = "Nhà ở", GioiHanTien = 0, TienDaTieu = 0, Mau = "#FFCE56",DanhGia = "Tot", IdNganSach = budget.IdNganSach },
                new Danhmuc { TenDanhMuc = "Giải trí", GioiHanTien = 0, TienDaTieu = 0, Mau = "#4BC0C0",DanhGia = "Tot", IdNganSach = budget.IdNganSach },
                new Danhmuc { TenDanhMuc = "Học tập", GioiHanTien = 0, TienDaTieu = 0, Mau = "#9966FF",DanhGia = "Tot", IdNganSach = budget.IdNganSach },
                new Danhmuc { TenDanhMuc = "Khác", GioiHanTien = 0, TienDaTieu = 0, Mau = "#FF9F40",DanhGia = "Tot", IdNganSach = budget.IdNganSach }
            };

            _categoryRepo.AddRange(defaultCategories);
            _categoryRepo.Save();

            return true;
        }

        public Ngansach? GetBudgetByMonth(int accid, int month, int year)
        {
            return _repo.GetByMonth(accid, month, year);
        }
    }
}
