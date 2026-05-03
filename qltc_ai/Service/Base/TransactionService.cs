using qltc_ai.Models;
using qltc_ai.Repositories;

namespace qltc_ai.Service.Base
{
    public class TransactionService : ITransactionService
    {
        private readonly ICategoryRepository _categoryRepo;
        private readonly IBudgetRepository _budgetRepo;
        private readonly ITransactionRepository _transactionRepo;
        private readonly ICategoryService _categoryService;

        public TransactionService(
            ICategoryRepository categoryRepo,
            IBudgetRepository budgetRepo,
            ITransactionRepository transactionRepo,
            ICategoryService categoryService)
        {
            _categoryRepo = categoryRepo;
            _budgetRepo = budgetRepo;
            _transactionRepo = transactionRepo;
            _categoryService = categoryService;
        }

        public List<Giaodich> GetByAccount(int accId)
        {
            return _transactionRepo.GetByAccount(accId);
        }

        public List<Giaodich> GetByAccountAndMonth(int accId, int month, int year)
        {
            return _transactionRepo.GetByAccountAndMonth(accId, month, year);
        }

        public bool AddTransaction(int accId, int idDetail, decimal money, string note, string typeTran)
        {
            if (money <= 0)
                return false;
            var now = DateTime.Now;
            ChiTietDanhMuc? detail;
            Ngansach? budget;

            if (typeTran == "ThuNhap")
            {
                
                budget = _budgetRepo.GetByMonth(accId, now.Month, now.Year);
                if (budget == null)
                    return false;

               
                detail = _categoryRepo.GetByBudgetT(budget.IdNganSach);
                if (detail == null)
                    return false;
            }
            else
            {
                
                detail = _categoryRepo.FindById(idDetail);
                if (detail == null)
                    return false;

                budget = _budgetRepo.FindById(detail.IdNganSach ?? 0);
                if (budget == null || budget.IdTaiKhoan != accId)
                    return false;

                if (budget.Thang?.Month != now.Month || budget.Thang?.Year != now.Year)
                    return false;
            }

            var cate = detail.IdDanhMucNavigation;
            if (cate == null)
                return false;

            if(typeTran != cate.LoaiDanhMuc)
                return false;

            
            if (cate.LoaiDanhMuc == "ChiTieu")
            {
                decimal daTieu = detail.TienDaTieu ?? 0;
                decimal gioiHan = detail.GioiHanTien ?? 0;

                if (gioiHan > 0 && daTieu + money > gioiHan)
                    return false;

                detail.TienDaTieu = daTieu + money;
                
               
            }
            else if(cate.LoaiDanhMuc == "ThuNhap")
            {
                budget.TongTien = (budget.TongTien ?? 0) + money;
                detail.TienDaTieu = (detail.TienDaTieu ?? 0) + money;
            }

            _categoryService.Rating(detail);

            var gd = new Giaodich
            {
                IdTaiKhoan = accId,
                IdChiTiet = detail.IdChiTiet,
                Tien = money,
                NoiDung = note,
                LoaiGiaoDich = typeTran,
                NgayGiaoDich = now
            };

            _transactionRepo.AddTransaction(gd);

            _categoryRepo.UpdateCategory(detail);
            _budgetRepo.UpdateBudget(budget);

            _transactionRepo.Save();
            _categoryRepo.Save();

            return true;
        }

        public bool UpdateTransaction(int accId, int idTran, decimal newMoney, string newNote)
        {
            if (newMoney <= 0)
                return false;

            var tran = _transactionRepo.FindById(idTran);
            if (tran == null || tran.IdTaiKhoan != accId)
                return false;

            var chiTiet = _categoryRepo.FindById(tran.IdChiTiet ?? 0);
            if (chiTiet == null)
                return false;

           
            if (tran.LoaiGiaoDich == "ChiTieu")
            {
                decimal diff = newMoney - (tran.Tien ?? 0);
                decimal newDaTieu = (chiTiet.TienDaTieu ?? 0) + diff;
                decimal gioiHan = chiTiet.GioiHanTien ?? 0;

                if (gioiHan > 0 && newDaTieu > gioiHan)
                    return false;

                chiTiet.TienDaTieu = newDaTieu;
                _categoryService.Rating(chiTiet);
                _categoryRepo.UpdateCategory(chiTiet);
            }

            tran.Tien = newMoney;
            tran.NoiDung = newNote;

            _transactionRepo.Update(tran);
            _transactionRepo.Save();
            _categoryRepo.Save();

            return true;
        }

        public bool DeleteTransaction(int idTran)
        {
            var tran = _transactionRepo.FindById(idTran);
            if (tran == null)
                return false;

            
            if (tran.LoaiGiaoDich == "ChiTieu" && tran.IdChiTiet.HasValue)
            {
                var chiTiet = _categoryRepo.FindById(tran.IdChiTiet.Value);
                if (chiTiet != null)
                {
                    chiTiet.TienDaTieu = Math.Max(0, (chiTiet.TienDaTieu ?? 0) - (tran.Tien ?? 0));
                    _categoryService.Rating(chiTiet);
                    _categoryRepo.UpdateCategory(chiTiet);
                    _categoryRepo.Save();
                }
            }

            _transactionRepo.DeleteTransaction(tran);
            _transactionRepo.Save();

            return true;
        }
    }
}
